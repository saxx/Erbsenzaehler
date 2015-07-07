using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Erbsenzaehler.AutoImporter.Configuration;
using Erbsenzaehler.AutoImporter.Recipies;
using Erbsenzaehler.Deduplicate;
using Erbsenzaehler.Importer;
using Erbsenzaehler.Models;
using Erbsenzaehler.Rules;
using Newtonsoft.Json;
using OneTrueError.Reporting;

namespace Erbsenzaehler.AutoImporter.WebJob
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = new Logger();

            try
            {
                logger.Info("Erbsenzaehler.AutoImporter.WebJob v" + typeof(Program).Assembly.GetName().Version + " starting up ...");

                var ignoreInterval = args.Any(x => x.Equals("--ignoreInterval", StringComparison.InvariantCultureIgnoreCase));

                // hard-code german culture here, we want our e-mails formatted for german
                var germanCulture = new CultureInfo("de-DE");
                Thread.CurrentThread.CurrentCulture = germanCulture;
                Thread.CurrentThread.CurrentUICulture = germanCulture;

                if (!string.IsNullOrEmpty(Config.OneTrueErrorAppKey) && !string.IsNullOrEmpty(Config.OneTrueErrorAppSecret))
                {
                    OneTrue.Configuration.Credentials(Config.OneTrueErrorAppKey, Config.OneTrueErrorAppSecret);
                }

                using (var db = new Db(Erbsenzaehler.Config.DatabaseConnectionString))
                {
                    var clients = db.Clients
                        .Where(x => x.AutoImporterSettings != null && x.AutoImporterSettings != "")
                        .Select(x => new
                        {
                            ClientId = x.Id,
                            ClientName = x.Name,
                            Settings = x.AutoImporterSettings,
                            Accounts = x.Accounts.Select(y => new
                            {
                                AccountId = y.Id,
                                AccountName = y.Name,
                                LastImport = y.ImportLogs.Where(z => z.Type == ImportLogType.AutomaticOnServer).Select(z => z.Date).DefaultIfEmpty().Max()
                            })
                        })
                        .ToList();

                    logger.Trace(clients.Count() + " client(s) found with auto import configuration.");
                    foreach (var client in clients)
                    {
                        try
                        {
                            logger.Trace("Parsing configuration for client #" + client.ClientId + " " + client.ClientName + " ...");
                            var configurations = ParseSettings(client.Settings).ToList();

                            var configCount = 0;
                            foreach (var config in configurations)
                            {
                                logger.Info("Running import " + ++configCount + " of " + configurations.Count() + " of client #" + client.ClientId + " " + client.ClientName + " ...");
                                var account = client.Accounts.FirstOrDefault(x => x.AccountName.Equals(config.Erbsenzaehler.Account, StringComparison.InvariantCultureIgnoreCase));
                                if (account == null)
                                {
                                    throw new Exception("There is no account '" + config.Erbsenzaehler.Account + "'.");
                                }

                                if (ignoreInterval || account.LastImport <= DateTime.UtcNow.AddMinutes(-Config.ImportIntervalInMinutes))
                                {
                                    if (ignoreInterval)
                                    {
                                        logger.Trace("Completely ignoring interval because of command-line argument.");
                                    }
                                    RunImport(db, config, client.ClientId, account.AccountId, logger).Wait();
                                }
                                else
                                {
                                    logger.Trace("Import for account '" + account.AccountName + "' already happened at " + account.LastImport + " UTC.");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.ToString());
                            LogException(ex);
                        }
                    }
                }

                logger.Info("Everything done. Goodbye.");
            }
            catch (Exception ex)
            {
                logger.Fatal(ex.ToString());
                LogException(ex);
            }
        }


        private static IEnumerable<ConfigurationContainer> ParseSettings(string settings)
        {
            return JsonConvert.DeserializeObject<IEnumerable<ConfigurationContainer>>(settings);
        }


        private static async Task RunImport(Db db, ConfigurationContainer config, int clientId, int accountId, ILogger logger)
        {
            var watch = new Stopwatch();
            watch.Start();

            var importLog = new ImportLog
            {
                Date = DateTime.UtcNow,
                AccountId = accountId,
                Type = ImportLogType.AutomaticOnServer,
                Milliseconds = (int)watch.ElapsedMilliseconds
            };

            try
            {
                var tempFilePath = RunRecipe(config, logger);

                if (File.Exists(tempFilePath) && new FileInfo(tempFilePath).Length > 0)
                {
                    var importResult = await ParseAndSaveToDatabase(db, clientId, accountId, tempFilePath, config, logger);
                    importLog.LinesDuplicatesCount = importResult.DuplicateLinesCount;
                    importLog.LinesFoundCount = importResult.NewLinesCount + importResult.DuplicateLinesCount;
                    importLog.LinesImportedCount = importResult.NewLinesCount;
                }
                else
                {
                    throw new Exception("File (to import from) does not exist or is empty.");
                }

                if (File.Exists(tempFilePath))
                {
                    logger?.Trace("Deleting temporary file {0} ...", tempFilePath);
                    File.Delete(tempFilePath);
                }
            }
            catch (Exception ex)
            {
                importLog.Log = ex.Message;
                logger?.Error(ex.ToString());
                LogException(ex);
            }
            finally
            {
                logger?.Trace("Saving to import log ...");

                watch.Stop();
                importLog.Milliseconds = (int)watch.ElapsedMilliseconds;

                db.ImportLog.Add(importLog);
                db.SaveChanges();
            }
        }


        private static string RunRecipe(ConfigurationContainer config, ILogger logger)
        {
            var tempFilePath = Path.GetTempFileName();
            logger?.Trace("Temporary file path is {0} ...", tempFilePath);

            var recipe = RecipeFactory.GetRecipe(config);
            recipe.DownloadFile(tempFilePath, logger);

            return tempFilePath;
        }


        private static async Task<ImporterBase.ImportResult> ParseAndSaveToDatabase(Db db, int clientId, int accountId, string tempFilePath, ConfigurationContainer config, ILogger logger)
        {
            logger?.Trace("Parsing file and saving to database ...");
            logger?.Trace("File size: " + new FileInfo(tempFilePath).Length + " bytes.");

            ImporterBase.ImportResult importResult;

            var importerType = (ImporterType)Enum.Parse(typeof(ImporterType), config.Erbsenzaehler.Importer);
            using (var reader = new StreamReader(tempFilePath, Encoding.UTF8))
            {
                var concreteImporter = new ImporterFactory().GetImporter(reader, importerType);
                importResult = await concreteImporter.LoadFileAndImport(db, clientId, accountId, new RulesApplier());

                logger?.Info(importResult.NewLinesCount + " line(s) created, " + importResult.DuplicateLinesCount + " duplicate line(s).");
            }

            return importResult;
        }


        private static void LogException(Exception ex)
        {
            if (!string.IsNullOrEmpty(Config.OneTrueErrorAppKey) && !string.IsNullOrEmpty(Config.OneTrueErrorAppSecret))
            {
                OneTrue.Report(ex);
            }
        }
    }
}
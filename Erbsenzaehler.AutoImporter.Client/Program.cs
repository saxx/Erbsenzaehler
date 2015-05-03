using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Erbsenzaehler.AutoImporter.Client.Uploader;
using Erbsenzaehler.AutoImporter.Configuration;
using Erbsenzaehler.AutoImporter.Recipies;
using Newtonsoft.Json;

namespace Erbsenzaehler.AutoImporter.Client
{
    public static class Program
    {
        private static void Main()
        {
            var logger = new Logger();

            try
            {
                logger.Info("Erbsenzähler AutoImporter starting ...");

                #region Configuration

                logger.Trace("Looking for configuration file ...");
                var configPath = Path.Combine(Environment.CurrentDirectory, "config.json");
                if (!File.Exists(configPath))
                {
                    logger.Fatal("Unable to locate configuration file at " + configPath + ".");
                    Environment.Exit(-1);
                }

                logger.Trace("Loading configuration ...");
                try
                {
                    Configuration = JsonConvert.DeserializeObject<IEnumerable<ConfigurationContainer>>(File.ReadAllText(configPath)).ToList();
                }
                catch (Exception ex)
                {
                    logger.Fatal("Unable to parse configuration file: ");
                    logger.Fatal(ex.Message);
                    Environment.Exit(-2);
                }

                #endregion

                var configCount = 0;
                foreach (var config in Configuration)
                {
                    logger.Info("Running import " + ++configCount + " of " + Configuration.Count() + " ...");

                    var tempFilePath = Path.GetTempFileName();
                    logger.Trace("Temporary file path is {0} ...", tempFilePath);

                    var recipe = RecipeFactory.GetRecipe(config);
                    recipe.DownloadFile(tempFilePath, logger);

                    if (File.Exists(tempFilePath) && new FileInfo(tempFilePath).Length > 0)
                    {
                        var uploader = new ErbsenzaehlerUploader(config.Erbsenzaehler);
                        uploader.Upload(tempFilePath, logger);
                    }

                    if (File.Exists(tempFilePath))
                    {
                        logger.Trace("Deleting temporary file {0} ...", tempFilePath);
                        File.Delete(tempFilePath);
                    }
                }

                logger.Info("Quitting ...");
            }
            catch (Exception ex)
            {
                logger.Fatal(ex.ToString());
            }
        }


        private static IList<ConfigurationContainer> Configuration { get; set; }
    }
}
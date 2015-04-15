using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Erbsenzaehler.AutoImporter.Client.Recipies;
using Erbsenzaehler.AutoImporter.Client.Uploader;
using Erbsenzaehler.AutoImporter.Configuration;
using Newtonsoft.Json;
using NLog;

namespace Erbsenzaehler.AutoImporter.Client
{
    public static class Program
    {
        private static void Main()
        {
            try
            {
                Log.Info("Erbsenzähler AutoImporter starting ...");

                #region Configuration

                Log.Trace("Looking for configuration file ...");
                var configPath = Path.Combine(Environment.CurrentDirectory, "config.json");
                if (!File.Exists(configPath))
                {
                    Log.Fatal("Unable to locate configuration file at " + configPath + ".");
                    Environment.Exit(-1);
                }

                Log.Trace("Loading configuration ...");
                try
                {
                    Configuration = JsonConvert.DeserializeObject<IEnumerable<ConfigurationContainer>>(File.ReadAllText(configPath)).ToList();
                }
                catch (Exception ex)
                {
                    Log.Fatal("Unable to parse configuration file: ");
                    Log.Fatal(ex.Message);
                    Environment.Exit(-2);
                }

                #endregion

                var configCount = 0;
                foreach (var config in Configuration)
                {
                    Log.Info("Running import " + ++configCount + " of " + Configuration.Count() + " ...");

                    var tempFilePath = Path.GetTempFileName();
                    Log.Trace("Temporary file path is {0} ...", tempFilePath);

                    var recipe = RecipeFactory.GetRecipe(config);
                    recipe.DownloadFile(tempFilePath);

                    if (File.Exists(tempFilePath) && new FileInfo(tempFilePath).Length > 0)
                    {
                        var uploader = new ErbsenzaehlerUploader(config.Erbsenzaehler);
                        uploader.Upload(tempFilePath);
                    }

                    if (File.Exists(tempFilePath))
                    {
                        Log.Trace("Deleting temporary file {0} ...", tempFilePath);
                        File.Delete(tempFilePath);
                    }
                }

                Log.Info("Quitting ...");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.ToString);
            }
        }


        private static Logger Log => LogManager.GetCurrentClassLogger();

        private static IList<ConfigurationContainer> Configuration { get; set; }
    }
}
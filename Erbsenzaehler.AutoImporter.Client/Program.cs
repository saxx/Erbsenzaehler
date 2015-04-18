using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Erbsenzaehler.AutoImporter.Recipies;
using Erbsenzaehler.AutoImporter.Client.Uploader;
using Erbsenzaehler.AutoImporter.Configuration;
using Newtonsoft.Json;

namespace Erbsenzaehler.AutoImporter.Client
{
    public static class Program
    {
        private static void Main()
        {
            try
            {
                Console.WriteLine("Erbsenzähler AutoImporter starting ...");

                #region Configuration

                Console.WriteLine("Looking for configuration file ...");
                var configPath = Path.Combine(Environment.CurrentDirectory, "config.json");
                if (!File.Exists(configPath))
                {
                    Console.WriteLine("Unable to locate configuration file at " + configPath + ".");
                    Environment.Exit(-1);
                }

                Console.WriteLine("Loading configuration ...");
                try
                {
                    Configuration = JsonConvert.DeserializeObject<IEnumerable<ConfigurationContainer>>(File.ReadAllText(configPath)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to parse configuration file: ");
                    Console.WriteLine(ex.Message);
                    Environment.Exit(-2);
                }

                #endregion

                var configCount = 0;
                foreach (var config in Configuration)
                {
                    Console.WriteLine("Running import " + ++configCount + " of " + Configuration.Count() + " ...");

                    var tempFilePath = Path.GetTempFileName();
                    Console.WriteLine("Temporary file path is {0} ...", tempFilePath);

                    var recipe = RecipeFactory.GetRecipe(config);
                    recipe.DownloadFile(tempFilePath);

                    if (File.Exists(tempFilePath) && new FileInfo(tempFilePath).Length > 0)
                    {
                        var uploader = new ErbsenzaehlerUploader(config.Erbsenzaehler);
                        uploader.Upload(tempFilePath);
                    }

                    if (File.Exists(tempFilePath))
                    {
                        Console.WriteLine("Deleting temporary file {0} ...", tempFilePath);
                        File.Delete(tempFilePath);
                    }
                }

                Console.WriteLine("Quitting ...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        private static IList<ConfigurationContainer> Configuration { get; set; }
    }
}
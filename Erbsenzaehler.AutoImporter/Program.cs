using System;
using System.IO;
using CommandLine;
using Erbsenzaehler.AutoImporter.Recipies;
using Erbsenzaehler.AutoImporter.Uploader;
using NLog;

namespace Erbsenzaehler.AutoImporter
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Log.Info("Erbsenzähler AutoImporter starting ...");

            #region Configuration

            Configuration = new Configuration();

            Log.Trace("Loading configuration from app settings ...");
            try
            {
                Configuration.LoadFromAppSettings();
            }
            catch (Exception ex)
            {
                Log.Fatal("Unable to parse app settings:");
                Log.Fatal(ex.Message);
                Environment.Exit(-1);
            }

            Log.Trace("Loading configuration from environment variables ...");
            try
            {
                Configuration.LoadFromEnvironmentVariables();
            }
            catch (Exception ex)
            {
                Log.Fatal("Unable to parse environment variables:");
                Log.Fatal(ex.Message);
                Environment.Exit(-1);
            }

            Log.Trace("Loading configuration from command line ...");
            if (!Parser.Default.ParseArguments(args, Configuration))
            {
                Log.Fatal("Unable to parse commandline parameters.");
                Environment.Exit(-1);
            }

            #endregion

            var tempFilePath = Path.GetTempFileName();
            Log.Trace("Temporary file path is {0} ...", tempFilePath);

            if (Configuration.Recipe == "Easybank")
            {
                var recipe = new EasybankRecipe(
                    Configuration.EasybankUsername,
                    Configuration.EasybankPassword,
                    Configuration.EasybankAccount);

                recipe.DownloadFile(tempFilePath);
            }

            if (File.Exists(tempFilePath) && new FileInfo(tempFilePath).Length > 0)
            {
                var uploader = new ErbsenzaehlerUploader(
                    Configuration.ErbsenzaehlerUsername,
                    Configuration.ErbsenzaehlerPassword,
                    Configuration.ErbsenzaehlerAccount
                    );

                if (!string.IsNullOrWhiteSpace(Configuration.ErbsenzaehlerUrl))
                {
                    uploader.BaseUrl = Configuration.ErbsenzaehlerUrl;
                }

                uploader.Upload(tempFilePath);

                Log.Trace("Deleting file {0} ...", tempFilePath);
                File.Delete(tempFilePath);
            }

            Log.Info("Quitting ...");
        }


        private static Logger Log => LogManager.GetCurrentClassLogger();

        private static Configuration Configuration { get; set; }
    }
}
using System;
using System.Configuration;
using CommandLine;
using CommandLine.Text;

namespace Erbsenzaehler.AutoImporter
{
    public class Configuration
    {
        public Configuration()
        {
            Recipe = "Easybank";
        }


        [Option("ErbsenzaehlerUrl", Required = false, HelpText = "The URL to Erbsenzähler. Set this value only if your Erbsenzähler URL differs from the default.")]
        public string ErbsenzaehlerUrl { get; set; }

        [Option("ErbsenzaehlerUsername", Required = false, HelpText = "The username for your Erbsenzähler account.")]
        public string ErbsenzaehlerUsername { get; set; }

        [Option("ErbsenzaehlerPassword", Required = false, HelpText = "The password for your Erbsenzähler account.")]
        public string ErbsenzaehlerPassword { get; set; }

        [Option("ErbsenzaehlerAccount", Required = false,
            HelpText = "The Erbsenzähler account into that the account statements should be imported.")]
        public string ErbsenzaehlerAccount { get; set; }

        [Option("Recipe", Required = false, HelpText = "The import recipe to run.")]
        public string Recipe { get; set; }


        [Option("EasybankUsername", Required = false, HelpText = "The username of your Easybank account.")]
        public string EasybankUsername { get; set; }

        [Option("EasybankPassword", Required = false, HelpText = "The password of your Easybank account.")]
        public string EasybankPassword { get; set; }

        [Option("EasybankAccount", Required = false,
            HelpText = "The account number of your Easybank account that should be imported into Erbsenzähler.")]
        public string EasybankAccount { get; set; }


        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }


        public void LoadFromEnvironmentVariables()
        {
            foreach (var property in GetType().GetProperties())
            {
                var envVar = Environment.GetEnvironmentVariable("Erbsenzaehler.AutoImporter." + property.Name);
                if (!string.IsNullOrWhiteSpace(envVar))
                {
                    property.SetValue(this, envVar);
                }
            }
        }


        public void LoadFromAppSettings()
        {
            foreach (var property in GetType().GetProperties())
            {
                var appSetting =
                    ConfigurationManager.AppSettings["Erbsenzaehler.AutoImporter." + property.Name];
                if (!string.IsNullOrWhiteSpace(appSetting))
                {
                    property.SetValue(this, appSetting);
                }
            }
        }
    }
}
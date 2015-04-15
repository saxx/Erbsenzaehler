using System.Configuration;

namespace Erbsenzaehler.AutoImporter.WebJob
{
    public static class Config
    {
        public static string ErbsenzaehlerUrl => ConfigurationManager.AppSettings["Erbsenzaehler.Url"] ?? "http://erbsenzaehler.azurewebsites.net";
        public static string OneTrueErrorAppKey => ConfigurationManager.AppSettings["OneTrueError.AppKey"] ?? "";
        public static string OneTrueErrorAppSecret => ConfigurationManager.AppSettings["OneTrueError.AppSecret"] ?? "";
    }
}
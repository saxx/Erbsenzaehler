using System.Configuration;

namespace Erbsenzaehler.SummaryMails.WebJob
{
    public static class Config
    {
        public static string ErbsenzaehlerUrl => ConfigurationManager.AppSettings["Erbsenzaehler.Url"] ?? "http://erbsenzaehler.azurewebsites.net";
        public static string OneTrueErrorAppKey => ConfigurationManager.AppSettings["OneTrueError.AppKey"] ?? "";
        public static string OneTrueErrorAppSecret => ConfigurationManager.AppSettings["OneTrueError.AppSecret"] ?? "";
        public static string SendGridUserName => ConfigurationManager.AppSettings["SendGrid.UserName"] ?? "";
        public static string SendGridPassword => ConfigurationManager.AppSettings["SendGrid.Password"] ?? "";
        public static string SendGridSender => ConfigurationManager.AppSettings["SendGrid.Sender"] ?? "Erbsenzähler <erbsenzaehler@erbsenzaehler.azurewebsites.net>";
    }
}
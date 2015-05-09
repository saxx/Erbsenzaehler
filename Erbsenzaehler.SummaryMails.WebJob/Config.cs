namespace Erbsenzaehler.SummaryMails.WebJob
{
    public static class Config
    {
        public static string OneTrueErrorAppKey => Erbsenzaehler.Config.Setting("OneTrueError.AppKey", "");
        public static string OneTrueErrorAppSecret => Erbsenzaehler.Config.Setting("OneTrueError.AppSecret", "");
        public static string SendGridUserName => Erbsenzaehler.Config.Setting("SendGrid.UserName", "");
        public static string SendGridPassword => Erbsenzaehler.Config.Setting("SendGrid.Password", "");
        public static string SendGridSender => Erbsenzaehler.Config.Setting("SendGrid.Sender", "Erbsenzähler <erbsenzaehler@erbsenzaehler.sachsenhofer.com>");
    }
}
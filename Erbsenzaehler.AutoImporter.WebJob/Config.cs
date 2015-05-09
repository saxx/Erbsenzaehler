namespace Erbsenzaehler.AutoImporter.WebJob
{
    public static class Config
    {
        public static string OneTrueErrorAppKey => Erbsenzaehler.Config.Setting("OneTrueError.AppKey", "");
        public static string OneTrueErrorAppSecret => Erbsenzaehler.Config.Setting("OneTrueError.AppSecret", "");
        public static int ImportIntervalInMinutes => Erbsenzaehler.Config.Setting("ImportIntervalInMinutes", 60*3);
    }
}
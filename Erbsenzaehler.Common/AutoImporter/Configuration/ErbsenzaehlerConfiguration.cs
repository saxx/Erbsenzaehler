namespace Erbsenzaehler.AutoImporter.Configuration
{
    public class ErbsenzaehlerConfiguration
    {
        public ErbsenzaehlerConfiguration()
        {
            Url = "http://erbsenzaehler.azurewebsites.net";
            Importer = "Easybank";
        }


        public string Username { get; set; }
        public string Password { get; set; }
        public string Account { get; set; }
        public string Importer { get; set; }
        public string Url { get; set; }
    }
}
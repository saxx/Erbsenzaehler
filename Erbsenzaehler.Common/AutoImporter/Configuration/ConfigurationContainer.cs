namespace Erbsenzaehler.AutoImporter.Configuration
{
    public class ConfigurationContainer
    {
        public bool SaveScreenshots { get; set; }
        public ErbsenzaehlerConfiguration Erbsenzaehler { get; set; }
        public EasybankConfiguration Easybank { get; set; }
    }
}
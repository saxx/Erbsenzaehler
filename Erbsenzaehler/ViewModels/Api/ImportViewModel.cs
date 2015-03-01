
namespace Erbsenzaehler.ViewModels.Api
{
    public class ImportViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Account { get; set; }
        public string Importer { get; set; }
        public byte[] File { get; set; }
    }
}
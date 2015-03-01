using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using NLog;

namespace Erbsenzaehler.AutoImporter.Uploader
{
    public class ErbsenzaehlerUploader
    {

        public string Username { get; set; }
        public string Password { get; set; }
        public string BaseUrl { get; set; } = "http://erbsenzaehler.azurewebsites.net/";
        public string Account { get; set; }
        public string Importer { get; set; } = "Easybank";

        public ErbsenzaehlerUploader(string username, string password, string account)
        {
            Username = username;
            Password = password;
            Account = account;
        }

        public void Upload(string filePath)
        {
            Log.Info("Uploading file to Erbsenzähler ...");
            Log.Trace("Erbsenzähler base URL is {0}.", BaseUrl);

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };

            Log.Trace("Building POST content ...");
            var postContent = new ObjectContent(typeof(object), new
            {
                Username,
                Password,
                Account,
                Importer,
                File = System.IO.File.ReadAllBytes(filePath)
            }, new JsonMediaTypeFormatter());

            Log.Trace("Uploading ...");
            var postResult = httpClient.PostAsync("Api/Import", postContent).Result;
            try
            {
                postResult.EnsureSuccessStatusCode();
            }
            catch
            {
                Log.Error("Unable to upload to Erbsenzähler: " + postResult.StatusCode + " " + postResult.ReasonPhrase);
                return;
            }

            var importResult = JsonConvert.DeserializeObject<dynamic>(postResult.Content.ReadAsStringAsync().Result);
            Log.Info("Upload completed. {0} lines imported, {1} lines ignored.", importResult.ImportedCount, importResult.IgnoredCount);
        }

        private static Logger Log => LogManager.GetCurrentClassLogger();
    }
}

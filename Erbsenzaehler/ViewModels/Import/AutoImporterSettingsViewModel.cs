using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using Erbsenzaehler.AutoImporter.Configuration;
using Erbsenzaehler.ExtensionMethods;
using Erbsenzaehler.Models;
using Newtonsoft.Json;

namespace Erbsenzaehler.ViewModels.Import
{
    public class AutoImporterSettingsViewModel
    {
        public AutoImporterSettingsViewModel Fill(Client client)
        {
            Settings = client.AutoImporterSettings;
            return this;
        }


        public async Task<AutoImporterSettingsViewModel> Save(Db db, Client client)
        {
            var clientInDb = await db.Clients.FirstOrDefaultAsync(x => x.Id == client.Id);

            // see if the string is valid JSON
            try
            {
                JsonConvert.DeserializeObject<IEnumerable<ConfigurationContainer>>(Settings);
                clientInDb.AutoImporterSettings = Settings.NullIfEmpty();
                await db.SaveChangesAsync();

                SavedSuccessfully = true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            return this;
        }


        public string ErrorMessage { get; set; }
        public bool SavedSuccessfully { get; set; }

        public string Settings { get; set; }
    }
}
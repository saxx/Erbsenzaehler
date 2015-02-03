using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Erbsenzaehler.Importer;
using Erbsenzaehler.Models;

namespace Erbsenzaehler.ViewModels.Import
{
    public class IndexViewModel
    {
        public async Task<IndexViewModel> Fill(Db db, Client client)
        {
            AvailableAccounts = (await db.Accounts.Where(x => x.ClientId == client.Id).OrderBy(x => x.Name).ToListAsync()).Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString(CultureInfo.InvariantCulture)
            });

            AvailableImporters = new[]
            {
                new SelectListItem
                {
                    Text = "Easybank",
                    Value = ImporterType.Easybank.ToString()
                },
                new SelectListItem
                {
                    Text = "TSV",
                    Value = ImporterType.Tsv.ToString()
                }
            };
            return this;
        }


        public IndexViewModel PreSelect(int accountId, ImporterType importer)
        {
            var selectedAccount = AvailableAccounts.FirstOrDefault(x => x.Value == accountId.ToString(CultureInfo.InvariantCulture));
            if (selectedAccount != null)
                selectedAccount.Selected = true;

            var selectedImporter = AvailableImporters.FirstOrDefault(x => x.Value == importer.ToString());
            if (selectedImporter != null)
                selectedImporter.Selected = true;

            return this;
        }


        public IEnumerable<SelectListItem> AvailableAccounts { get; set; }
        public IEnumerable<SelectListItem> AvailableImporters { get; set; }

        public ImporterBase.ImportResult ImportResult { get; set; }
    }
}
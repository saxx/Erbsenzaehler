using System;
using System.Data.Entity;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Erbsenzaehler.Importer;
using Erbsenzaehler.Rules;
using Erbsenzaehler.ViewModels.Import;

namespace Erbsenzaehler.Controllers
{
    [Authorize]
    public class ImportController : ControllerBase
    {
        public async Task<ActionResult> Index()
        {
            var currentClient = await GetCurrentClient();
            return View(await new IndexViewModel().Fill(Db, currentClient));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(HttpPostedFileBase file, int accountId, ImporterType importer)
        {
            var currentClient = await GetCurrentClient();
            var account = await Db.Accounts.FirstOrDefaultAsync(x => x.Id == accountId && x.ClientId == currentClient.Id);
            var viewModel = (await new IndexViewModel().Fill(Db, currentClient)).PreSelect(accountId, importer);

            try
            {
                if (account != null && file != null)
                {
                    using (var reader = new StreamReader(file.InputStream, Encoding.Default))
                    {
                        var concreteImporter = new ImporterFactory().GetImporter(reader, importer);
                        viewModel.ImportResult = await concreteImporter.LoadFileAndImport(Db, currentClient, account, new RulesApplier());
                    }
                }
            }
            catch (Exception ex)
            {
                viewModel.ErrorMessage = ex.Message;
            }

            return View(viewModel);
        }
    }
}
using System.Data.Entity;
using System.IO;
using System.Net;
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
            if (account == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var viewModel = (await new IndexViewModel().Fill(Db, currentClient)).PreSelect(accountId, importer);

            using (var reader = new StreamReader(file.InputStream, Encoding.Default))
            {
                var concreteImporter = new ImporterFactory().GetImporter(reader, importer);
                viewModel.ImportResult = await concreteImporter.LoadFileAndImport(Db, currentClient, account, new RulesApplier());
            }

            return View(viewModel);
        }
    }
}
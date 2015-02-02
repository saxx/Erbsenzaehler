using System.Data.Entity;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Erbsenzaehler.Importer;
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
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var tempPath = Path.GetTempFileName();
            file.SaveAs(tempPath);

            //var importService = new CsvImportService();
            var viewModel = (await new IndexViewModel().Fill(Db, currentClient)).PreSelect(accountId, importer);
            //viewModel.LinesCount = await importService.RunImportAndSaveLinesToDatabase(await GetCurrentClient(), tempPath, account, Db, importer);

            System.IO.File.Delete(tempPath);

            return View(viewModel);
        }
    }
}
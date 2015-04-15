using System;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Erbsenzaehler.Importer;
using Erbsenzaehler.Models;
using Erbsenzaehler.Rules;
using Erbsenzaehler.ViewModels.Import;

namespace Erbsenzaehler.Controllers
{
    [Authorize]
    public class ImportController : ControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }


        #region AutoImporterSettings
        public async Task<ActionResult> AutoImporterSettings()
        {
            var viewModel = new AutoImporterSettingsViewModel().Fill(await GetCurrentClient());
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AutoImporterSettings(AutoImporterSettingsViewModel viewModel)
        {
            await viewModel.Save(Db, await GetCurrentClient());
            return View(viewModel);
        }
        #endregion


        #region Manual Import
        public async Task<ActionResult> ManualImport()
        {
            var currentClient = await GetCurrentClient();
            return View(await new ManualImportViewModel().Fill(Db, currentClient));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManualImport(HttpPostedFileBase file, int accountId, ImporterType importer)
        {
            var watch = new Stopwatch();
            var currentUser = await GetCurrentUser();
            var currentClient = await GetCurrentClient();
            var account = await Db.Accounts.FirstOrDefaultAsync(x => x.Id == accountId && x.ClientId == currentClient.Id);
            var viewModel = (await new ManualImportViewModel().Fill(Db, currentClient)).PreSelect(accountId, importer);

            watch.Start();
            try
            {
                if (account != null && file != null)
                {
                    using (var reader = new StreamReader(file.InputStream, Encoding.Default))
                    {
                        var concreteImporter = new ImporterFactory().GetImporter(reader, importer);
                        viewModel.ImportResult = await concreteImporter.LoadFileAndImport(Db, currentClient.Id, account.Id, new RulesApplier());

                        watch.Stop();

                        // save to import log
                        Db.ImportLog.Add(new ImportLog
                        {
                            AccountId = account.Id,
                            UserId = currentUser.Id,
                            Date = DateTime.UtcNow,
                            LinesDuplicatesCount = viewModel.ImportResult.DuplicateLinesCount,
                            LinesFoundCount = viewModel.ImportResult.DuplicateLinesCount,
                            LinesImportedCount = viewModel.ImportResult.NewLinesCount,
                            Type = ImportLogType.Manual,
                            Milliseconds = (int)watch.ElapsedMilliseconds,
                            Log = null
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                viewModel.ErrorMessage = ex.Message;
            }

            return View(viewModel);
        }
        #endregion
    }
}
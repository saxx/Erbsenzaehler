using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Erbsenzaehler.Rules;
using Erbsenzaehler.ViewModels.Rules;
using Newtonsoft.Json;

namespace Erbsenzaehler.Controllers
{
    [Authorize]
    public class ManageRulesController : ControllerBase
    {
        public ActionResult Index()
        {
            var viewModel = new IndexViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Apply()
        {
            var viewModel = new IndexViewModel();

            var currentClient = await GetCurrentClient();
            var applier = new RulesApplier();

            var lines = Db.Lines.Where(x => !x.UpdatedManually);

            viewModel.ApplierResult = await applier.Apply(Db, currentClient, lines, true);
            if (viewModel.ApplierResult.LinesUpdated > 0)
            {
                await Db.SaveChangesAsync();
            }

            return View("Index", viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Upload(HttpPostedFileBase file)
        {
            var viewModel = new IndexViewModel();

            if (file != null)
            {
                var importer = new RulesImporter();

                using (var reader = new StreamReader(file.InputStream, Encoding.Default))
                {
                    var json = reader.ReadToEnd();
                    viewModel.ImportResult = await importer.Import(Db, await GetCurrentClient(), json);
                }
            }

            return View("Index", viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Download()
        {
            var currentClient = await GetCurrentClient();
            var rules = await Db.Rules.Where(x => x.ClientId == currentClient.Id)
                .OrderBy(x => x.Regex)
                .Select(x => new
                {
                    x.Regex,
                    x.ChangeCategoryTo,
                    ChangeDateTo = x.ChangeDateTo.HasValue ? x.ChangeDateTo.Value.ToString() : null,
                    x.ChangeIgnoreTo
                }).ToListAsync();

            var json = JsonConvert.SerializeObject(rules, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore

            });

            return File(Encoding.Default.GetBytes(json), "text/json", "rules.json");
        }
    }
}
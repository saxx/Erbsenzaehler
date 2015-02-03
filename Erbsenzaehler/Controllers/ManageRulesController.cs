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
                .OrderBy(x => x.Id)
                .Select(x => new
                {
                    x.ChangeCategoryTo,
                    x.ChangeDateTo,
                    x.ChangeIgnoreTo,
                    x.Regex,
                    x.Id
                }).ToListAsync();

            var json = JsonConvert.SerializeObject(rules, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            return File(Encoding.Default.GetBytes(json), "text/json", "rules.json");
        }
    }
}
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Erbsenzaehler.Models;
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
            return View(new IndexViewModel());
        }

        #region Apply

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Apply()
        {
            var viewModel = new IndexViewModel();

            var currentClient = await GetCurrentClient();
            var applier = new RulesApplier();

            var lines = Db.Lines;

            viewModel.ApplierResult = await applier.Apply(Db, currentClient, lines, true);
            if (viewModel.ApplierResult.LinesUpdated > 0)
            {
                await Db.SaveChangesAsync();
            }

            return View("Index", viewModel);
        }

        #endregion

        #region Upload

        public ActionResult Upload()
        {
            return View(new UploadRulesViewModel());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Upload(HttpPostedFileBase file)
        {
            var viewModel = new UploadRulesViewModel();

            if (file != null)
            {
                var importer = new RulesImporter();

                using (var reader = new StreamReader(file.InputStream, Encoding.UTF8))
                {
                    var json = reader.ReadToEnd();
                    viewModel.Result = await importer.Import(Db, await GetCurrentClient(), json);
                }
            }

            return View(viewModel);
        }

        #endregion

        #region Download

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

        #endregion

        #region Json 

        public async Task<ActionResult> Json()
        {
            var currentClient = await GetCurrentClient();
            var rules = (await Db.Rules
                .Where(x => x.ClientId == currentClient.Id)
                .OrderBy(x => x.Regex)
                .ToListAsync())
                .Select(x => new JsonRule(x));
            return base.Json(rules, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public async Task<ActionResult> Json(JsonRule jsonRule)
        {
            var currentClient = await GetCurrentClient();

            var rule = new Rule
            {
                Regex = jsonRule.regex,
                ChangeCategoryTo = jsonRule.category,
                ChangeIgnoreTo = jsonRule.ignore,
                ChangeDateTo = jsonRule.date,
                ClientId = currentClient.Id
            };
            Db.Rules.Add(rule);
            await Db.SaveChangesAsync();

            return base.Json(new JsonRule(rule));
        }


        [HttpPut]
        public async Task<ActionResult> Json(int id, JsonRule jsonRule)
        {
            var currentClient = await GetCurrentClient();

            var rule = await Db.Rules.FirstOrDefaultAsync(x => x.ClientId == currentClient.Id && x.Id == id);
            if (rule == null)
            {
                // if rule does not exist, create new one
                return await Json(jsonRule);
            }

            rule.Regex = jsonRule.regex;
            rule.ChangeCategoryTo = jsonRule.category;
            rule.ChangeIgnoreTo = jsonRule.ignore;
            rule.ChangeDateTo = jsonRule.date;
            await Db.SaveChangesAsync();

            return base.Json(new JsonRule(rule));
        }


        [HttpDelete]
        public async Task<ActionResult> Json(int id)
        {
            var currentClient = await GetCurrentClient();

            var rule = await Db.Rules.FirstOrDefaultAsync(x => x.ClientId == currentClient.Id && x.Id == id);
            if (rule != null)
            {
                Db.Rules.Remove(rule);
                await Db.SaveChangesAsync();
            }

            return base.Json(new JsonRule(rule));
        }


        public class JsonRule
        {
            public JsonRule()
            {
            }


            public JsonRule(Rule rule)
            {
                id = rule.Id;
                regex = rule.Regex;
                category = rule.ChangeCategoryTo;
                ignore = rule.ChangeIgnoreTo;
                date = rule.ChangeDateTo;
            }


            // ReSharper disable InconsistentNaming
            public int id { get; set; }
            public string regex { get; set; }
            public string category { get; set; }
            public bool? ignore { get; set; }
            public Rule.ChangeDateToOption? date { get; set; }
            // ReSharper restore InconsistentNaming
        }

        #endregion
    }
}
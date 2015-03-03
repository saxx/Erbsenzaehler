using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Erbsenzaehler.Models;
using Erbsenzaehler.ViewModels.ManageBudgets;

namespace Erbsenzaehler.Controllers
{
    [Authorize]
    public class ManageBudgetsController : ControllerBase
    {
        public async Task<ActionResult> Index()
        {
            var viewModel = await new IndexViewModel().Fill(Db, await GetCurrentClient());
            return View(viewModel);
        }

        #region Json 

        public async Task<ActionResult> Json()
        {
            var currentClient = await GetCurrentClient();
            var rules = (await Db.Budgets
                .Where(x => x.ClientId == currentClient.Id)
                .OrderBy(x => x.Category)
                .ToListAsync())
                .Select(x => new JsonBudget(x));
            return base.Json(rules, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public async Task<ActionResult> Json(JsonBudget jsonBudget)
        {
            var currentClient = await GetCurrentClient();

            var budget = new Budget
            {
                Category = jsonBudget.category,
                Limit = jsonBudget.limit,
                Period = jsonBudget.period,
                ClientId = currentClient.Id
            };
            Db.Budgets.Add(budget);
            await Db.SaveChangesAsync();

            return base.Json(new JsonBudget(budget));
        }


        [HttpPut]
        public async Task<ActionResult> Json(int id, JsonBudget jsonBudget)
        {
            var currentClient = await GetCurrentClient();

            var budget = await Db.Budgets.FirstOrDefaultAsync(x => x.ClientId == currentClient.Id && x.Id == id);
            if (budget == null)
            {
                // if rule does not exist, create new one
                return await Json(jsonBudget);
            }

            budget.Category = jsonBudget.category;
            budget.Limit = jsonBudget.limit;
            budget.Period = jsonBudget.period;
            await Db.SaveChangesAsync();

            return base.Json(new JsonBudget(budget));
        }


        [HttpDelete]
        public async Task<ActionResult> Json(int id)
        {
            var currentClient = await GetCurrentClient();

            var budget = await Db.Budgets.FirstOrDefaultAsync(x => x.ClientId == currentClient.Id && x.Id == id);
            if (budget != null)
            {
                Db.Budgets.Remove(budget);
                await Db.SaveChangesAsync();
            }

            return base.Json(new JsonBudget(budget));
        }


        public class JsonBudget
        {
            public JsonBudget()
            {
            }


            public JsonBudget(Budget budget)
            {
                id = budget.Id;
                category = budget.Category;
                limit = budget.Limit;
                period = budget.Period;
            }


            // ReSharper disable InconsistentNaming
            public int id { get; set; }
            public string category { get; set; }
            public decimal limit { get; set; }
            public Budget.LimitPeriod period { get; set; }
            // ReSharper restore InconsistentNaming
        }

        #endregion
    }
}
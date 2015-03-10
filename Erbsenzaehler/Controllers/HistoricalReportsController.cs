using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Erbsenzaehler.ViewModels.HistoricalReports;

namespace Erbsenzaehler.Controllers
{
    public class HistoricalReportsController : ControllerBase
    {
        public async Task<ActionResult> Index()
        {
            var viewModel = await (new IndexViewModel()).Calculate(await GetCurrentClient(), Db);
            return View(viewModel);
        }


        public async Task<ActionResult> Budget(string month)
        {
            var yearValue = DateTime.Now.Year;
            var monthValue = DateTime.Now.Month;

            try
            {
                if (!string.IsNullOrEmpty(month))
                {
                    yearValue = int.Parse(month.Split('-')[0]);
                    monthValue = int.Parse(month.Split('-')[1]);
                }
            }
            catch
            {
                // do nothing here, we already set a useful default value above.
            }

            var viewModel = await (new BudgetViewModel()).Fill(Db, await GetCurrentClient(), yearValue, monthValue);
            return View(viewModel);
        }


        public async Task<ActionResult> Balance()
        {
            var viewModel = await (new BalanceViewModel()).Fill(Db, await GetCurrentClient());
            return View(viewModel);
        }


        public async Task<ActionResult> Amounts()
        {
            var viewModel = await (new AmountsViewModel()).Fill(Db, await GetCurrentClient());
            return View(viewModel);
        }
    }
}
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Erbsenzaehler.ViewModels.HistoricalReports;

namespace Erbsenzaehler.Controllers
{
    [Authorize]
    public class HistoricalReportsController : ControllerBase
    {
        public async Task<ActionResult> Index()
        {
            var viewModel = await (new IndexViewModel()).Calculate(await GetCurrentClient(), Db);
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
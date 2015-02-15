using System.Threading.Tasks;
using System.Web.Mvc;
using Erbsenzaehler.ViewModels.Reports;

namespace Erbsenzaehler.Controllers
{
    public class ReportsController : ControllerBase
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
    }
}
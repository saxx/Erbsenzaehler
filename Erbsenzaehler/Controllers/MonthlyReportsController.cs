using System.Threading.Tasks;
using System.Web.Mvc;
using Erbsenzaehler.ViewModels.MonthlyReports;

namespace Erbsenzaehler.Controllers
{
    [Authorize]
    public class MonthlyReportsController : ControllerBase
    {
        public async Task<ActionResult> Index(string month)
        {
            var viewModel = await new IndexViewModel().Fill(Db, await GetCurrentClient(), month);
            return View(viewModel);
        }

    }
}
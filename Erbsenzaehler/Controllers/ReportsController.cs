using Erbsenzaehler.ViewModels.Reports;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Erbsenzaehler.Controllers
{
    public class ReportsController : ControllerBase
    {
        public async Task<ActionResult> Index()
        {
            var viewModel = new IndexViewModel().Calculate(await GetCurrentClient(), Db);

            return View(viewModel);
        }
	}
}
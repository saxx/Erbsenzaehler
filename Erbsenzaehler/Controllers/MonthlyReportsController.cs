using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Erbsenzaehler.Models;
using Erbsenzaehler.Reporting;
using Erbsenzaehler.ViewModels.MonthlyReports;

namespace Erbsenzaehler.Controllers
{
    [Authorize]
    public class MonthlyReportsController : ControllerBase
    {
        public async Task<ActionResult> Index(string month)
        {
            var viewModel = await new IndexViewModel().Fill(Db, await GetCurrentClient(), new Month(month));
            return View(viewModel);
        }


        #region JSON
        public async Task<ActionResult> SpendingsChart(string month)
        {
            var calculator = new SumCalculator(Db, await GetCurrentClient());

            var result = from x in await calculator.CalculateForMonth(new Month(month))
                         where x.Key != Constants.IncomeCategory
                         orderby x.Value
                         select new
                         {
                             amount = -x.Value,
                             category = x.Key
                         };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
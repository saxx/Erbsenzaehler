using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Erbsenzaehler.Models;
using Erbsenzaehler.Reporting;
using Erbsenzaehler.ViewModels.Today;

namespace Erbsenzaehler.Controllers
{
    [Authorize]
    public class TodayController : ControllerBase
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


        public async Task<ActionResult> BalanceChart(string month)
        {
            var calculator = new SumCalculator(Db, await GetCurrentClient());
            var monthParam = new Month(month);

            var incomes = new Dictionary<Month, decimal>();
            var spendings = new Dictionary<Month, decimal>();

            var months = new List<Month>
            {
                monthParam.IsCurrentMonth ? monthParam.PreviousMonth.PreviousMonth.PreviousMonth.PreviousMonth : monthParam.PreviousMonth.PreviousMonth.PreviousMonth,
                monthParam.IsCurrentMonth ? monthParam.PreviousMonth.PreviousMonth.PreviousMonth : monthParam.PreviousMonth.PreviousMonth,
                monthParam.IsCurrentMonth ? monthParam.PreviousMonth.PreviousMonth : monthParam.PreviousMonth,
                monthParam.IsCurrentMonth ? monthParam.PreviousMonth : monthParam,
                monthParam.IsCurrentMonth ? monthParam : monthParam.NextMonth
            };

            foreach (var m in months)
            {
                var calcResult = (await calculator.CalculateForMonth(m)).ToList();
                incomes[m] = calcResult.Where(x => x.Key == Constants.IncomeCategory).Select(x => x.Value).Sum();
                spendings[m] = calcResult.Where(x => x.Key != Constants.IncomeCategory).Select(x => x.Value).Sum();
            }

            var result = from x in months
                orderby x.Date
                select new
                {
                    month = x.Date.ToString("MMM yyyy"),
                    income = incomes[x],
                    spendings = -spendings[x]
                };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> BudgetChart(string month)
        {
            var calculator = new BudgetCalculator(Db, await GetCurrentClient());

            var selectedMonth = new Month(month);
            var budgets = await calculator.CalculateForMonth(selectedMonth);

            IEnumerable<BudgetCalculator.BudgetResult> relativeBudgets = null;
            if (selectedMonth.IsCurrentMonth)
            {
                relativeBudgets = await calculator.CalculateRelativeToCurrentMonth();
            }


            var result = from x in budgets
                orderby x.Category
                select new
                {
                    percentage = x.Percentage,
                    relativePercentage = relativeBudgets == null ? null : new int?(relativeBudgets.First(y => y.Category == x.Category).Percentage),
                    category = x.Category
                };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
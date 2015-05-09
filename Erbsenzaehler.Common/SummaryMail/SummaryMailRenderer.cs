using System;
using System.Threading.Tasks;
using Erbsenzaehler.Models;
using Erbsenzaehler.Reporting;
using Erbsenzaehler.Templates;

namespace Erbsenzaehler.SummaryMail
{
    public class SummaryMailRenderer
    {
        #region Constructor
        private readonly Db _db;
        private readonly BudgetCalculator _budgetCalculator;
        private readonly SumCalculator _sumCalculator;


        public SummaryMailRenderer(
            Db db,
            BudgetCalculator budgetCalculator,
            SumCalculator sumCalculator)
        {
            _sumCalculator = sumCalculator;
            _budgetCalculator = budgetCalculator;
            _db = db;
        }

        #endregion

        public async Task<string> Render(User user)
        {
            var model = await new SummaryMailModel(_db, _budgetCalculator, _sumCalculator).Fill(user);
            var renderer = new RazorRenderService<SummaryMailModel>();
            return renderer.Render("SummaryMail.cshtml", model);
        }
    }
}
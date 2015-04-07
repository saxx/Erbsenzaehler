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

        private readonly Uri _erbsenzaehlerUri;
        private readonly Db _db;
        private readonly BudgetCalculator _budgetCalculator;
        private readonly SumCalculator _sumCalculator;


        public SummaryMailRenderer(
            Db db,
            Uri erbsenzaehlerUri,
            BudgetCalculator budgetCalculator,
            SumCalculator sumCalculator)
        {
            _sumCalculator = sumCalculator;
            _budgetCalculator = budgetCalculator;
            _db = db;
            _erbsenzaehlerUri = erbsenzaehlerUri;
        }

        #endregion

        public async Task<string> Render(User user)
        {
            var model = await new SummaryMailModel().Fill(_db, user, _erbsenzaehlerUri, _budgetCalculator, _sumCalculator);
            var renderer = new RazorRenderService<SummaryMailModel>();
            return renderer.Render("SummaryMail.cshtml", model);
        }
    }
}
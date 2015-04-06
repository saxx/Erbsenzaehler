using System;
using System.IO;
using System.Threading.Tasks;
using Erbsenzaehler.Models;
using Erbsenzaehler.Reporting;
using RazorEngine.Configuration;
using RazorEngine.Templating;

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

            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "SummaryMail.cshtml");
            if (!File.Exists(templatePath))
            {
                templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "Templates", "SummaryMail.cshtml");
            }

            if (!File.Exists(templatePath))
            {
                throw new Exception("Template 'SummaryMail.cshtml' missing. It should be at '" + templatePath + "'.");
            }

            var template = File.ReadAllText(templatePath);

#pragma warning disable 618
            var razorConfig = new TemplateServiceConfiguration
            {
                DisableTempFileLocking = true
            };
            var razorService = RazorEngineService.Create(razorConfig);
            var html = razorService.RunCompile(template, "SummaryMail", typeof (SummaryMailModel), model);
#pragma warning restore 618

            return html;
        }
    }
}

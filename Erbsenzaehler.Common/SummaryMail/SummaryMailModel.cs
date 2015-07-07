using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Erbsenzaehler.ExtensionMethods;
using Erbsenzaehler.Models;
using Erbsenzaehler.Reporting;

namespace Erbsenzaehler.SummaryMail
{
    public class SummaryMailModel
    {
        #region Constructor

        private readonly Db _db;
        private readonly BudgetCalculator _budgetCalculator;
        private readonly SumCalculator _sumCalculator;


        public SummaryMailModel(
            Db db,
            BudgetCalculator budgetCalculator,
            SumCalculator sumCalculator)
        {
            _sumCalculator = sumCalculator;
            _budgetCalculator = budgetCalculator;
            _db = db;
        }

        #endregion

        public async Task<SummaryMailModel> Fill(
            User currentUser)
        {
            const int numberOfMonths = 3;

            CurrentDate = DateTime.UtcNow.ToShortDateString();
            ErbsenzaehlerUrl = new Uri(Config.ErbsenzaehlerUrl);

            // date and time of last summary mail sent to the user
            var lastSummaryMailDate = _db.SummaryMailLogs.ByUser(currentUser).Select(x => x.Date).DefaultIfEmpty().Max().Date;
            // if the last summary mail was long ago, just assume it was last month
            if (lastSummaryMailDate < DateTime.UtcNow.Date.AddMonths(-1))
            {
                lastSummaryMailDate = DateTime.UtcNow.Date.AddMonths(-1);
            }

            var client = currentUser.Client;
            ClientName = client.Name;

            var linesQuery = _db.Lines
                .ByClient(client)
                .ByNotIgnored();

            var latestLinesQuery = linesQuery
                .Where(x => x.DateOfCreationUtc >= lastSummaryMailDate)
                .OrderByDescending(x => x.Date ?? x.OriginalDate);
            Lines = (await latestLinesQuery.ToListAsync()).Select(x => new Line(x));

            Months = new List<string>();
            Budgets = new List<IEnumerable<BudgetCalculator.BudgetResult>>();
            Spendings = new List<IDictionary<string, decimal>>();
            Balances = new List<decimal>();

            var month = new Month();
            for (var i = 1; i <= numberOfMonths; i++)
            {
                Months.Insert(0, DateTime.UtcNow.AddMonths(-i + 1).ToString("MMMM yyyy"));
                Budgets.Insert(0, await _budgetCalculator.CalculateForMonth(month));
                Spendings.Insert(0, await _sumCalculator.CalculateForMonth(month));
                Balances.Insert(0, await linesQuery.ByMonth(month).Select(x => x.Amount ?? x.OriginalAmount).DefaultIfEmpty().SumAsync());

                month = month.PreviousMonth;
            }

            CurrentMonthRelativeBudget = await _budgetCalculator.CalculateRelativeToCurrentMonth();
            SpendingCategories = Spendings.SelectMany(x => x.Keys).ToList().Distinct().OrderBy(x => x).ToList();

            var importsQuery = from x in _db.ImportLog.ByClient(client).ByNoError()
                group x by x.Account
                into g
                select new
                {
                    Account = g.Key.Name,
                    LastImportDate = g.Select(y => y.Date).DefaultIfEmpty().Max()
                };
            LastImports = importsQuery.ToDictionary(x => x.Account, x => x.LastImportDate.ToRelativeDate());

            return this;
        }


        public IEnumerable<Line> Lines { get; set; }
        public Uri ErbsenzaehlerUrl { get; set; }
        public string CurrentDate { get; set; }
        public string ClientName { get; set; }

        public IList<string> Months { get; set; }

        public IList<IEnumerable<BudgetCalculator.BudgetResult>> Budgets { get; set; }
        public IEnumerable<BudgetCalculator.BudgetResult> CurrentMonthRelativeBudget { get; set; }

        public IEnumerable<string> SpendingCategories { get; set; }
        public IList<IDictionary<string, decimal>> Spendings { get; set; }

        public IList<decimal> Balances { get; set; }

        public IDictionary<string, string> LastImports { get; set; }

        public class Line
        {
            public Line(Models.Line line)
            {
                Date = (line.Date ?? line.OriginalDate).ToShortDateString();
                Text = (line.Text ?? line.OriginalText) ?? "";
                Amount = (line.Amount ?? line.OriginalAmount).ToString("N2");
                Category = line.Category ?? "";
            }


            public string Date { get; set; }
            public string Text { get; set; }
            public string Amount { get; set; }
            public string Category { get; set; }
        }
    }
}
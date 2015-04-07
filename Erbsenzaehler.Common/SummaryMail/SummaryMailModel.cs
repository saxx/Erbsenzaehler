using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Erbsenzaehler.Models;
using Erbsenzaehler.Reporting;

namespace Erbsenzaehler.SummaryMail
{
    public class SummaryMailModel
    {
        public async Task<SummaryMailModel> Fill(
            Db db,
            User currentUser,
            Uri erbsenzaehlerUri,
            BudgetCalculator budgetCalculator,
            SumCalculator sumCalculator)
        {
            CurrentMonth = DateTime.UtcNow.ToString("MMMM yyyy");
            LastMonth = DateTime.UtcNow.AddMonths(-1).ToString("MMMM yyyy");
            CurrentDate = DateTime.UtcNow.ToShortDateString();
            ErbsenzaehlerUrl = erbsenzaehlerUri;

            // date and time of last summary mail sent to the user
            var lastSummaryMailDate = db.SummaryMailLogs.ByUser(currentUser).Select(x => x.Date).DefaultIfEmpty().Max().Date;
            // if the last summary mail was long ago, just assume it was last month
            if (lastSummaryMailDate < DateTime.UtcNow.Date.AddMonths(-1))
            {
                lastSummaryMailDate = DateTime.UtcNow.Date.AddMonths(-1);
            }

            var client = currentUser.Client;
            ClientName = client.Name;

            var linesQuery = db.Lines
                .ByClient(client)
                .ByNotIgnored();

            var latestLinesQuery = linesQuery
                .Where(x => (x.Date ?? x.OriginalDate) >= lastSummaryMailDate)
                .OrderByDescending(x => x.Date ?? x.OriginalDate);
            Lines = (await latestLinesQuery.ToListAsync()).Select(x => new Line(x));

            var currentMonth = new Month();
            CurrentMonthBudget = await budgetCalculator.CalculateForMonth(currentMonth);
            LastMonthBudget = await budgetCalculator.CalculateForMonth(currentMonth.PreviousMonth);

            CurrentMonthSpendings = await sumCalculator.CalculateForMonth(currentMonth);
            LastMonthSpendings = await sumCalculator.CalculateForMonth(currentMonth.PreviousMonth);

            var categories = CurrentMonthSpendings.Select(x => x.Key).ToList();
            categories.AddRange(LastMonthSpendings.Keys);
            SpendingCategories = categories.Distinct().OrderBy(x => x).ToList();

            var currentMonthLinesQuery = linesQuery.ByMonth(currentMonth);
            var lastMonthLinesQuery = linesQuery.ByMonth(currentMonth.PreviousMonth);
            CurrentMonthBalance = await currentMonthLinesQuery.Select(x => x.Amount ?? x.OriginalAmount).DefaultIfEmpty().SumAsync();
            LastMonthBalance = await lastMonthLinesQuery.Select(x => x.Amount ?? x.OriginalAmount).DefaultIfEmpty().SumAsync();

            return this;
        }


        public IEnumerable<Line> Lines { get; set; }
        public Uri ErbsenzaehlerUrl { get; set; }
        public string CurrentDate { get; set; }
        public string CurrentMonth { get; set; }
        public string LastMonth { get; set; }
        public string ClientName { get; set; }

        public IEnumerable<BudgetCalculator.BudgetResult> CurrentMonthBudget { get; set; }
        public IEnumerable<BudgetCalculator.BudgetResult> LastMonthBudget { get; set; }

        public IEnumerable<string> SpendingCategories { get; set; }
        public IDictionary<string, decimal> CurrentMonthSpendings { get; set; }
        public IDictionary<string, decimal> LastMonthSpendings { get; set; }

        public decimal CurrentMonthBalance { get; set; }
        public decimal LastMonthBalance { get; set; }

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
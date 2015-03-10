using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Erbsenzaehler.Models;

namespace Erbsenzaehler.Reporting
{
    public class BudgetCalculator
    {
        private readonly Db _db;
        private readonly Client _currentClient;

        public BudgetCalculator(Db db, Client currentClient)
        {
            _currentClient = currentClient;
            _db = db;
        }


        public async Task Calculate(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                throw new Exception("The start-date must not be larger than the end-date.");
            }

            // make sure we select entire days
            startDate = startDate.Date;
            endDate = endDate.Date.AddDays(1).AddSeconds(-1);

            StartDate = startDate;
            EndDate = endDate;

            var allBudgets = await _db.Budgets.Where(x => x.ClientId == _currentClient.Id && x.Category != null && x.Category != "")
                .OrderBy(x => x.Category)
                .ToListAsync();

            var allLines = await _db.Lines.Where(x => x.Account.ClientId == _currentClient.Id && x.Category != null && x.Category != "")
                .Where(x => (x.Date ?? x.OriginalDate) >= startDate && (x.Date ?? x.OriginalDate) <= endDate)
                .Select(x =>
                    new
                    {
                        Amount = x.Amount ?? x.OriginalAmount,
                        x.Category
                    }).ToListAsync();

            var days = (int)Math.Ceiling((EndDate - StartDate).TotalDays);
            var result = new List<BudgetResult>();
            foreach (var budget in allBudgets)
            {
                var budgetResult = new BudgetResult(budget.Category, days);
                var budgetCategory = budget.Category.ToLower();

                budgetResult.TotalAmount = allLines.Where(x => x.Category.ToLower() == budgetCategory).Select(x => x.Amount).Sum();
                budgetResult.TotalLimit = budget.LimitInDays * days;

                result.Add(budgetResult);
            }

            BudgetResults = result;
        }


        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        public IEnumerable<BudgetResult> BudgetResults { get; private set; }


        public class BudgetResult
        {

            public BudgetResult(string category, int days)
            {
                Days = days;
                Category = category;
            }


            public string Category { get; set; }

            public int Days { get; set; }

            public decimal TotalLimit { get; set; }

            public decimal TotalAmount { get; set; }

            public int CurrentPercentage
            {
                get
                {
                    var i = (int)Math.Round(100 / TotalLimit * (TotalLimit + TotalAmount));

                    if (i < 100)
                    {
                        return 100 - i;
                    }

                    return 100 - i;
                }
            }
        }
    }
}
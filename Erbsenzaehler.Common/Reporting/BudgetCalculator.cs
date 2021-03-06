﻿using System;
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


        public async Task<IEnumerable<BudgetResult>> CalculateForMonth(Month month)
        {
            var allBudgets = await _db.Budgets
                .Where(x => x.ClientId == _currentClient.Id && x.Category != null && x.Category != "")
                .OrderBy(x => x.Category)
                .ToListAsync();

            var allLines = await _db.Lines.ByClient(_currentClient).ByMonth(month).ByNotIgnored().ByCategoryNotEmpty().Select(x => new
            {
                Amount = x.Amount ?? x.OriginalAmount,
                x.Category
            }).ToListAsync();

            var result = new List<BudgetResult>();
            foreach (var budget in allBudgets)
            {
                var budgetResult = new BudgetResult(budget.Category);
                var budgetCategory = budget.Category.ToLower();

                budgetResult.Amount = allLines.Where(x => x.Category.ToLower() == budgetCategory).Select(x => x.Amount).Sum();
                budgetResult.Limit = budget.NormalizeLimit(month);

                result.Add(budgetResult);
            }

            return result;
        }


        public async Task<IEnumerable<BudgetResult>> CalculateRelativeToCurrentMonth()
        {
            var currentMonth = new Month();
            var budgets = await CalculateForMonth(currentMonth);

            var daysHappened = currentMonth.NumberOfDays - currentMonth.NumberOfDaysLeft;
            var factor = 1.0d/daysHappened*currentMonth.NumberOfDays;
            foreach (var budget in budgets)
            {
                budget.Amount = budget.Amount*(decimal) factor;
            }

            return budgets;
        }


        public class BudgetResult
        {
            public BudgetResult(string category)
            {
                Category = category;
            }


            public string Category { get; set; }

            public decimal Limit { get; set; }

            public decimal Amount { get; set; }

            public int Percentage
            {
                get
                {
                    if (Limit <= 0)
                    {
                        return 0;
                    }

                    var i = (int) Math.Round(100/Limit*(Limit + Amount));

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
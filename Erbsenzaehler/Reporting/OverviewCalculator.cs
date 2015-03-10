using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Erbsenzaehler.Models;

namespace Erbsenzaehler.Reporting
{
    public class OverviewCalculator
    {
        private readonly Db _db;
        private readonly Client _client;
        public const string EmptyCategory = "Other";
        public const string IncomeCategory = "Income";


        public OverviewCalculator(Db db, Client client)
        {
            _client = client;
            _db = db;
        }


        public async Task<IEnumerable<string>> GetCategories(bool includeIncome = false)
        {
            var allCategories = await _db.Lines.Where(x => x.Account.ClientId == _client.Id && x.Category != null).Select(x => x.Category).Distinct().ToListAsync();
            allCategories.Add(EmptyCategory);
            if (includeIncome)
            {
                allCategories.Add(IncomeCategory);
            }
            return allCategories;
        }


        public async Task<IDictionary<Date, IDictionary<string, decimal>>> Calculate()
        {
            // for some reason it's dreadfully slow to do all the grouping in the database
            // it's much, much faster to do it in memory, even if this means that we require more memory for a short period of time
            // but I think it shouldn't matter that much because no client should have more than a few thousand lines

            var linesQuery = from x in _db.Lines
                where x.Account.ClientId == _client.Id && !x.Ignore
                select new
                {
                    x.Category,
                    Amount = x.Amount ?? x.OriginalAmount,
                    Date = x.Date ?? x.OriginalDate
                };
            var lines = await linesQuery.ToListAsync();

            var income = (from x in lines.Where(x => x.Category == null && x.Amount > 0)
                group x by new
                {
                    x.Date.Year,
                    x.Date.Month
                }
                into g
                select
                    new
                    {
                        g.Key.Year,
                        g.Key.Month,
                        Amount = g.Select(x => x.Amount).DefaultIfEmpty(0).Sum()
                    }).ToList();
            var spent = (from x in lines.Where(y => y.Category != null || y.Amount < 0)
                group x by new
                {
                    x.Category,
                    x.Date.Year,
                    x.Date.Month
                }
                into g
                select
                    new
                    {
                        g.Key.Category,
                        g.Key.Year,
                        g.Key.Month,
                        Amount = g.Select(x => x.Amount).DefaultIfEmpty(0).Sum()
                    }).ToList();

            var allCategories = (await GetCategories()).ToList();
            var result = new Dictionary<Date, IDictionary<string, decimal>>();


            // ReSharper disable PossibleMultipleEnumeration
            if (income.Any() || spent.Any())
            {
                var minYear = Math.Min(income.Select(x => x.Year).Min(), spent.Select(x => x.Year).Min());
                var maxYear = Math.Min(income.Select(x => x.Year).Max(), spent.Select(x => x.Year).Max());

                for (var year = minYear; year <= maxYear; year++)
                {
                    for (var month = 1; month <= 12; month++)
                    {
                        var yearClosure = year;
                        var monthClosure = month;
                        var date = new Date(yearClosure, monthClosure);
                        result[date] = new Dictionary<string, decimal>();

                        var filteredIncome = income.Where(x => x.Year == yearClosure && x.Month == monthClosure).ToList();
                        result[date][IncomeCategory] = filteredIncome.Select(x => x.Amount).DefaultIfEmpty(0).Sum();

                        var filteredSpent = spent.Where(x => x.Year == yearClosure && x.Month == monthClosure).ToList();
                        foreach (var category in allCategories)
                        {
                            result[date][category] = filteredSpent.Where(x => x.Category == category || (x.Category == null && category == EmptyCategory)).Select(x => x.Amount).DefaultIfEmpty(0).Sum();
                        }

                        // remove month if it does not have any value at all
                        if (result[date].All(x => x.Value == 0))
                        {
                            result.Remove(date);
                        }
                    }
                }
            }
            // ReSharper restore PossibleMultipleEnumeration
            return result;
        }
    }
}
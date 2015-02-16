using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Erbsenzaehler.Models;

namespace Erbsenzaehler.ViewModels.Reports
{
    public class AmountsViewModel
    {

        private const string EmptyCategory = "Sonstiges";
        public const string IncomeCategory = "Income";


        public async Task<AmountsViewModel> Fill(Db db, Client client)
        {
            var allCategories = await db.Lines.Where(x => x.Account.ClientId == client.Id && x.Category != null).Select(x => x.Category).Distinct().ToListAsync();
            allCategories.Add(EmptyCategory);
            allCategories.Add(IncomeCategory);

            var amounts = await (from x in db.Lines.Where(x => x.Account.ClientId == client.Id && !x.Ignore)
                                 group x by new { Category = x.Category ?? EmptyCategory, (x.Date ?? x.OriginalDate).Year, (x.Date ?? x.OriginalDate).Month }
                into g
                                 orderby g.Key.Year, g.Key.Month
                                 select new
                                 {
                                     g.Key.Category,
                                     g.Key.Year,
                                     g.Key.Month,
                                     Income = g.Where(y => y.OriginalAmount > 0 && y.Category == null).Select(y => y.OriginalAmount).DefaultIfEmpty(0).Sum(),
                                     Spent = g.Where(y => y.OriginalAmount < 0 || y.Category != null).Select(y => y.OriginalAmount).DefaultIfEmpty(0).Sum()
                                 }).ToListAsync();

            var minDate = new Date(DateTime.MaxValue.Year, 12);
            var maxDate = new Date(DateTime.MinValue.Year, 1);

            AmountPerMonth = new Dictionary<string, IDictionary<Date, decimal>>();
            foreach (var amount in amounts)
            {
                var date = new Date(amount.Year, amount.Month);
                if (date < minDate)
                {
                    minDate = date;
                }
                if (date > maxDate)
                {
                    maxDate = date;
                }

                // the spendings
                if (!AmountPerMonth.ContainsKey(amount.Category))
                {
                    AmountPerMonth[amount.Category] = new Dictionary<Date, decimal>();
                }
                if (!AmountPerMonth[amount.Category].ContainsKey(date))
                {
                    AmountPerMonth[amount.Category][date] = 0;
                }
                AmountPerMonth[amount.Category][date] += amount.Spent;

                // the income 
                if (!AmountPerMonth.ContainsKey(IncomeCategory))
                {
                    AmountPerMonth[IncomeCategory] = new Dictionary<Date, decimal>();
                }
                if (!AmountPerMonth[IncomeCategory].ContainsKey(date))
                {
                    AmountPerMonth[IncomeCategory][date] = 0;
                }
                AmountPerMonth[IncomeCategory][date] += amount.Income;
            }

            // now fill all the missing dates, so that every series has the exact same length
            foreach (var category in AmountPerMonth.Keys.ToList())
            {
                var currentDate = minDate;
                while (currentDate <= maxDate)
                {
                    if (!AmountPerMonth[category].ContainsKey(currentDate))
                    {
                        AmountPerMonth[category][currentDate] = 0;
                    }

                    currentDate += 1;
                }

                AmountPerMonth[category] = AmountPerMonth[category].OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            }




            AmountPerMonth = AmountPerMonth.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

            return this;
        }


        public IDictionary<string, IDictionary<Date, decimal>> AmountPerMonth { get; set; }
    }
}
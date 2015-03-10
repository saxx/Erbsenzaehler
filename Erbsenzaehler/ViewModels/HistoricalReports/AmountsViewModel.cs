using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Erbsenzaehler.Models;
using Erbsenzaehler.Reporting;

namespace Erbsenzaehler.ViewModels.HistoricalReports
{
    public class AmountsViewModel
    {
        public async Task<AmountsViewModel> Fill(Db db, Client client)
        {
            var overviewCalculator = new OverviewCalculator(db, client);

            var overview = await overviewCalculator.Calculate();

            var minDate = new Date(DateTime.MaxValue.Year, 12);
            var maxDate = new Date(DateTime.MinValue.Year, 1);

            AmountPerMonth = new Dictionary<string, IDictionary<Date, decimal>>();

            foreach (var monthPair in overview)
            {
                var date = monthPair.Key;
                if (date < minDate)
                {
                    minDate = date;
                }
                if (date > maxDate)
                {
                    maxDate = date;
                }

                foreach (var categoryPair in monthPair.Value)
                {
                    if (!AmountPerMonth.ContainsKey(categoryPair.Key))
                    {
                        AmountPerMonth[categoryPair.Key] = new Dictionary<Date, decimal>();
                    }
                    if (!AmountPerMonth[categoryPair.Key].ContainsKey(date))
                    {
                        AmountPerMonth[categoryPair.Key][date] = 0;
                    }
                    AmountPerMonth[categoryPair.Key][date] += categoryPair.Value;
                }
            }

            foreach (var category in AmountPerMonth.Keys.ToList())
            {
                AmountPerMonth[category] = AmountPerMonth[category].OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            }
            AmountPerMonth = AmountPerMonth.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

            return this;
        }


        public IDictionary<string, IDictionary<Date, decimal>> AmountPerMonth { get; set; }
        public static string IncomeCategory => OverviewCalculator.IncomeCategory;
    }
}
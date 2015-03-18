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
            var overviewCalculator = new OverviewCalculator(db, client, new SumCalculator(db, client));

            var overview = await overviewCalculator.Calculate();

            AmountPerMonth = new Dictionary<string, IDictionary<Month, decimal>>();

            foreach (var monthPair in overview)
            {
                foreach (var categoryPair in monthPair.Value)
                {
                    if (!AmountPerMonth.ContainsKey(categoryPair.Key))
                    {
                        AmountPerMonth[categoryPair.Key] = new Dictionary<Month, decimal>();
                    }
                    if (!AmountPerMonth[categoryPair.Key].ContainsKey(monthPair.Key))
                    {
                        AmountPerMonth[categoryPair.Key][monthPair.Key] = 0;
                    }
                    AmountPerMonth[categoryPair.Key][monthPair.Key] += categoryPair.Value;
                }
            }

            foreach (var category in AmountPerMonth.Keys.ToList())
            {
                AmountPerMonth[category] = AmountPerMonth[category].OrderBy(x => x.Key.Date).ToDictionary(x => x.Key, x => x.Value);
            }
            AmountPerMonth = AmountPerMonth.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

            return this;
        }


        public IDictionary<string, IDictionary<Month, decimal>> AmountPerMonth { get; set; }
        public static string IncomeCategory => Constants.IncomeCategory;
    }
}
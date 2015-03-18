using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Erbsenzaehler.Models;
using Erbsenzaehler.Reporting;

namespace Erbsenzaehler.ViewModels.HistoricalReports
{
    public class IndexViewModel
    {
        public async Task<IndexViewModel> Calculate(Client client, Db db)
        {
            var overviewCalculator = new OverviewCalculator(db, client, new SumCalculator(db, client));

            var allCategories = (await overviewCalculator.GetCategories()).ToList();
            Overview = new OverviewContainer
            {
                CategoryHeaders = allCategories
            };

            var overview = await overviewCalculator.Calculate();
            foreach (var monthPair in overview)
            {
                var monthContainer = new MonthContainer
                {
                    Year = monthPair.Key.Date.Year,
                    Month = monthPair.Key.Date.Month,
                    Name = monthPair.Key.ToString(),
                    Income = monthPair.Value.Where(x => x.Key == Constants.IncomeCategory).Sum(x => x.Value),
                    Spent = monthPair.Value.Where(x => x.Key != Constants.IncomeCategory).Sum(x => x.Value)
                };

                foreach (var category in allCategories)
                {
                    monthContainer[category] = monthPair.Value.ContainsKey(category) ? monthPair.Value[category] : 0;
                }

                if (monthContainer.Income > 0 || monthContainer.Spent < 0)
                {
                    Overview.Months.Insert(0, monthContainer);
                }
            }

            return this;
        }


        public OverviewContainer Overview { get; set; }


        public class OverviewContainer
        {
            public OverviewContainer()
            {
                CategoryHeaders = new List<string>();
                Months = new List<MonthContainer>();
            }


            public IList<string> CategoryHeaders { get; set; }
            public IList<MonthContainer> Months { get; set; }
        }

        public class MonthContainer : Dictionary<string, object>
        {
            public int Year
            {
                get
                {
                    return (int) this["Year"];
                }
                set
                {
                    this["Year"] = value;
                }
            }

            public int Month
            {
                get
                {
                    return (int) this["Month"];
                }
                set
                {
                    this["Month"] = value;
                }
            }

            public decimal Income
            {
                get
                {
                    return (decimal) this["Income"];
                }
                set
                {
                    this["Income"] = value;
                }
            }

            public decimal Spent
            {
                get
                {
                    return (decimal) this["Spent"];
                }
                set
                {
                    this["Spent"] = value;
                }
            }

            public string Name
            {
                get
                {
                    return this["Name"] as string;
                }
                set
                {
                    this["Name"] = value;
                }
            }

            public decimal Balance => Income + Spent;
        }
    }
}
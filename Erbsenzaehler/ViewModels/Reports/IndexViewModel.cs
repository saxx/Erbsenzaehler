using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Erbsenzaehler.Models;

namespace Erbsenzaehler.ViewModels.Reports
{
    public class IndexViewModel
    {
        private const string EmptyCategory = "Sonstiges";


        public async Task<IndexViewModel> Calculate(Client client, Db db)
        {
            var allCategories = await db.Lines.Where(x => x.Account.ClientId == client.Id && x.Category != null).Select(x => x.Category).Distinct().ToListAsync();
            allCategories.Add(EmptyCategory);

            var amountsQuery = from x in db.Lines.Where(x => x.Account.ClientId == client.Id && !x.Ignore)
                               group x by new
                               {
                                   Category = x.Category ?? EmptyCategory,
                                   (x.Date ?? x.OriginalDate).Year,
                                   (x.Date ?? x.OriginalDate).Month
                               } into g
                               orderby g.Key.Year, g.Key.Month
                               select
                           new
                           {
                               g.Key.Category,
                               g.Key.Year,
                               g.Key.Month,
                               Income = g.Where(y => (y.Amount ?? y.OriginalAmount) > 0 && y.Category == null).Select(y => (y.Amount ?? y.OriginalAmount)).DefaultIfEmpty(0).Sum(),
                               Spent = g.Where(y => (y.Amount ?? y.OriginalAmount) < 0 || y.Category != null).Select(y => (y.Amount ?? y.OriginalAmount)).DefaultIfEmpty(0).Sum()
                           };
            var amounts = await amountsQuery.ToListAsync();

            Overview = new OverviewContainer { CategoryHeaders = allCategories };

            if (amounts.Any())
            {
                for (var year = amounts.Select(x => x.Year).Min(); year <= amounts.Select(x => x.Year).Max(); year++)
                {
                    for (var month = 1; month <= 12; month++)
                    {
                        var yearClosure = year;
                        var monthClosure = month;

                        var filteredAmounts = amounts.Where(x => x.Year == yearClosure && x.Month == monthClosure).ToList();

                        // first, add up all the spent amounts
                        var monthContainer = new MonthContainer
                        {
                            Year = year,
                            Month = month,
                            Name = new DateTime(year, month, 1).ToString("MMM yyyy"),
                            Income = filteredAmounts.Sum(x => x.Income),
                            Spent = filteredAmounts.Sum(x => x.Spent)
                        };

                        foreach (var category in allCategories)
                        {
                            monthContainer[category] = filteredAmounts.Where(x => x.Category == category).Select(x => x.Spent).DefaultIfEmpty(0).Sum();
                        }


                        if (monthContainer.Income > 0 || monthContainer.Spent < 0)
                        {
                            Overview.Months.Insert(0, monthContainer);
                        }
                    }
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
                    return (int)this["Year"];
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
                    return (int)this["Month"];
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
                    return (decimal)this["Income"];
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
                    return (decimal)this["Spent"];
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
using Erbsenzaehler.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Erbsenzaehler.ViewModels.Reports
{
    public class IndexViewModel
    {
        private const string EmptyCategory = "Sonstiges";

        public IndexViewModel Calculate(Client client, Db db)
        {
            var allCategories = new[] { EmptyCategory };
            //db.Categories.Where(x => x.ClientId == client.Id).Select(x => x.Name).OrderBy(x => x).ToList();
            //allCategories.Add(EmptyCategory);

            /*var allRefunds = (from x in db.Lines.Where(x => x.Account.ClientId == client.Id && !x.Ignore)
                              select new
                              {
                                  Category = EmptyCategory,
                                  x.Amount
                              }).ToList();*/

            var amounts = (from x in db.Lines.Where(x => x.Account.ClientId == client.Id && !x.Ignore)
                           group x by new { x.Date.Year, x.Date.Month }
                               into g
                           orderby g.Key.Year, g.Key.Month
                           select new
                           {
                               g.Key.Year,
                               g.Key.Month,
                               Income = g.Where(y => y.Amount > 0).Select(y => y.Amount).DefaultIfEmpty(0).Sum(),
                               Spent = g.Where(y => y.Amount < 0).Select(y => y.Amount).DefaultIfEmpty(0).Sum()
                           }).ToList();

            Overview = new OverviewContainer { CategoryHeaders = allCategories };

            for (var year = amounts.Select(x => x.Year).Min(); year <= amounts.Select(x => x.Year).Max(); year++)
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
                        monthContainer[category] = filteredAmounts/*.Where(x => x.Category == category)*/.Select(x => x.Spent).DefaultIfEmpty(0).Sum();

                    /*
                    // now, substract the refunds
                    foreach (var refund in allRefunds.Where(x => x.RefundDate.Year == yearClosure && x.RefundDate.Month == monthClosure))
                    {
                        // the total sum
                        monthContainer.Spent += refund.Amount;
                        if (monthContainer.Spent > 0)
                        {
                            monthContainer.Income += monthContainer.Spent;
                            monthContainer.Spent = 0;
                        }

                        // for each category
                        var newCategoryAmount = (decimal)monthContainer[refund.Category];
                        newCategoryAmount += refund.Amount;
                        if (newCategoryAmount > 0)
                        {
                            monthContainer.Income += newCategoryAmount;
                            newCategoryAmount = 0;
                        }
                        monthContainer[refund.Category] = newCategoryAmount;
                    }*/

                    if (monthContainer.Income > 0 || monthContainer.Spent < 0)
                        Overview.Months.Insert(0, monthContainer);
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
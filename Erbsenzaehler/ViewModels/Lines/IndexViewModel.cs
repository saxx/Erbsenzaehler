using Erbsenzaehler.Migrations;
using Erbsenzaehler.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Erbsenzaehler.ViewModels.Lines
{
    public class IndexViewModel
    {
        public IndexViewModel()
        {
            Lines = new List<Line>();
        }

        public async Task<IndexViewModel> Fill(Client client, Db db, int? selectedYear, int? selectedMonth)
        {
            /*var uniqueDates = db.Lines
                .Where(x => x.ClientId == client.Id)
                .Select(x => x.Date)
                .Distinct()
                .ToList()
                .Select(x => new DateTime(x.Year, x.Month, 1))
                .Distinct()
                .ToList();

            if (selectedYear == null || selectedMonth == null)
            {
                var maxDate = uniqueDates.Select(x => x.Date).Max();
                selectedYear = maxDate.Year;
                selectedMonth = maxDate.Month;
            }

            var categories = await db.Categories.Where(x => x.ClientId == client.Id).Select(x => x.Name).OrderBy(x => x).ToListAsync();
            categories.Insert(0, "");
            AvailableCategories = categories;

            SelectedDate = selectedYear + "-" + selectedMonth;

            #region Lines
            var query = db.Lines
                .Where(x => x.ClientId == client.Id)
                .Include(x => x.Category)
                .Include(x => x.Account)
                .Where(x => x.Date.Year == selectedYear && x.Date.Month == selectedMonth)
                .OrderByDescending(x => x.Date);

            Lines = from x in (await query.ToListAsync())
                    select new Line
                    {
                        Account = x.Account.Name,
                        Amount = x.Amount.ToString("N2"),
                        Category = x.Category == null ? "" : x.Category.Name,
                        Date = x.Date.ToShortDateString(),
                        OriginalDate = x.OriginalDate.ToShortDateString(),
                        Id = x.Id,
                        Text = x.Text,
                        IsIgnored = x.Ignore,
                        RefundDate = x.RefundDate.HasValue ? x.RefundDate.Value.ToShortDateString() : ""
                    };

            AvailableDates = uniqueDates
                .OrderByDescending(x => x)
                .Select(x => new Month
                {
                    Value = x.Date.Year + "-" + x.Date.Month,
                    Name = x.Date.ToString("MMMM yyyy")
                }).ToList();
            #endregion

            #region Refunds
            var refundQuery = from line in db.Lines
                              where line.ClientId == client.Id &&
                                    line.RefundDate.HasValue &&
                                    line.RefundDate.Value.Year == selectedYear && line.RefundDate.Value.Month == selectedMonth &&
                                    (line.Date.Year != selectedYear || line.Date.Month != selectedMonth)
                              orderby line.RefundDate.Value
                              select new
                              {
                                  line.Amount,
                                  line.Date,
                                  RefundDate = line.RefundDate.Value,
                                  line.Text,
                                  Category = line.Category == null ? "" : line.Category.Name
                              };
            RefundsFromOtherMonths = (await refundQuery.ToListAsync()).Select(x => new Refund
            {
                LineAmount = x.Amount.ToString("N2"),
                LineDate = x.Date.ToShortDateString(),
                LineText = x.Text,
                LineCategory = x.Category,
                RefundDate = x.RefundDate.ToShortDateString()
            });
            #endregion*/

            return this;
        }

        public IEnumerable<Month> AvailableDates { get; set; }
        public IEnumerable<string> AvailableCategories { get; set; }
        public IEnumerable<Line> Lines { get; set; }

        public string SelectedDate { get; set; }

        public class Line
        {
            public string Account { get; set; }
            public string Date { get; set; }
            public string OriginalDate { get; set; }
            public string RefundDate { get; set; }
            public string Text { get; set; }
            public string Amount { get; set; }
            public string Category { get; set; }
            public int Id { get; set; }
            public bool IsIgnored { get; set; }
        }

        public class Refund
        {
            public string LineAmount { get; set; }
            public string LineDate { get; set; }
            public string LineText { get; set; }
            public string LineCategory { get; set; }
            public string RefundDate { get; set; }
        }

        public class Month
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }
}
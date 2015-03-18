using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Erbsenzaehler.Models;

namespace Erbsenzaehler.ViewModels.LinesEditor
{
    public class JsonViewModel
    {
        public JsonViewModel()
        {
            Lines = new List<Line>();
        }


        public async Task<JsonViewModel> Fill(Client client, Db db, int? selectedYear, int? selectedMonth)
        {
            FillDate(client, db, ref selectedYear, ref selectedMonth);
            await FillLines(client, db, selectedYear, selectedMonth);

            return this;
        }


        private void FillDate(Client client, Db db, ref int? selectedYear, ref int? selectedMonth)
        {
            var uniqueDates = db.Lines
                .Where(x => x.Account.ClientId == client.Id)
                .Select(x => x.Date ?? x.OriginalDate)
                .Distinct()
                .ToList()
                .Select(x => new DateTime(x.Year, x.Month, 1))
                .Distinct()
                .ToList();

            if (!uniqueDates.Any())
            {
                selectedYear = DateTime.Now.Year;
                selectedMonth = DateTime.Now.Month;
            }
            else if (selectedYear == null || selectedMonth == null)
            {
                var maxDate = uniqueDates.Select(x => x.Date).Max();
                selectedYear = maxDate.Year;
                selectedMonth = maxDate.Month;
            }

            SelectedDate = selectedYear + "-" + selectedMonth;

            AvailableDates = uniqueDates
                .OrderByDescending(x => x)
                .Select(x => new Month
                {
                    Value = x.Date.Year + "-" + x.Date.Month,
                    Name = x.Date.ToString("MMMM yyyy")
                }).ToList();
        }


        private async Task FillLines(Client client, Db db, int? selectedYear, int? selectedMonth)
        {
            var query = db.Lines
                .Where(x => x.Account.ClientId == client.Id)
                .Include(x => x.Account)
                .Where(x => (x.Date.HasValue && x.Date.Value.Year == selectedYear && x.Date.Value.Month == selectedMonth) ||
                            (!x.Date.HasValue && x.OriginalDate.Year == selectedYear && x.OriginalDate.Month == selectedMonth))
                .OrderByDescending(x => x.Date ?? x.OriginalDate);

            Lines = from x in (await query.ToListAsync())
                    select new Line
                    {
                        Account = x.Account.Name,
                        Amount = (x.Amount ?? x.OriginalAmount).ToString("N2"),
                        OriginalAmount = x.OriginalAmount.ToString("N2"),
                        Category = x.Category,
                        Date = (x.Date ?? x.OriginalDate).ToShortDateString(),
                        OriginalDate = x.OriginalDate.ToShortDateString(),
                        Id = x.Id,
                        Text = x.Text ?? x.OriginalText,
                        Ignore = x.Ignore,
                        ManuallyAdded = x.LineAddedManually
                    };
        }


        public IEnumerable<Month> AvailableDates { get; set; }
        public IEnumerable<Line> Lines { get; set; }

        public string SelectedDate { get; set; }

        public class Line
        {
            public string Account { get; set; }
            public string Date { get; set; }
            public string OriginalDate { get; set; }
            public string Text { get; set; }
            public string Amount { get; set; }
            public string OriginalAmount { get; set; }
            public string Category { get; set; }
            public int Id { get; set; }
            public bool Ignore { get; set; }
            public bool ManuallyAdded { get; set; }
        }

        public class Month
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }
}
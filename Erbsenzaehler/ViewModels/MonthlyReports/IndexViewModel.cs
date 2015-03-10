using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Erbsenzaehler.Models;

namespace Erbsenzaehler.ViewModels.MonthlyReports
{
    public class IndexViewModel
    {
        public async Task<IndexViewModel> Fill(Db db, Client currentClient, string month)
        {
            if (string.IsNullOrEmpty(month))
            {
                month = DateTime.Now.ToString("yyyy-MM");
            }

            if (month.IndexOf("-", StringComparison.InvariantCulture) != 4)
            {
                throw new Exception("'" + month + "' is not a valid month.");
            }

            var startDate = new DateTime(int.Parse(month.Substring(0, 4)), int.Parse(month.Substring(5)), 1);
            var endDate = startDate.AddMonths(1).AddSeconds(-1);

            var linesQuery = db.Lines.Where(x => x.Account.ClientId == currentClient.Id);

            CurrentDate = startDate;
            HasLines = await linesQuery.AnyAsync(x => (x.Date ?? x.OriginalDate) >= startDate && (x.Date ?? x.OriginalDate) <= endDate);
            MinDate = await linesQuery.Select(x => x.Date ?? x.OriginalDate).DefaultIfEmpty().MinAsync();
            MaxDate = await linesQuery.Select(x => x.Date ?? x.OriginalDate).DefaultIfEmpty().MaxAsync();

            return this;
        }

        public bool HasLines { get; set; }
        public DateTime CurrentDate { get; set; }
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }
    }
}
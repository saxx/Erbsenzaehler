using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Erbsenzaehler.Models;

namespace Erbsenzaehler.ViewModels.MonthlyReports
{
    public class IndexViewModel
    {
        public async Task<IndexViewModel> Fill(Db db, Client currentClient, Month month)
        {
            var startDate = month.Date;
            var endDate = startDate.AddMonths(1).AddSeconds(-1);

            var linesQuery = db.Lines.Where(x => x.Account.ClientId == currentClient.Id);

            CurrentDate = startDate;
            HasLines = await linesQuery.AnyAsync(x => (x.Date ?? x.OriginalDate) >= startDate && (x.Date ?? x.OriginalDate) <= endDate);
            MinDate = await linesQuery.Select(x => x.Date ?? x.OriginalDate).DefaultIfEmpty().MinAsync();
            MaxDate = await linesQuery.Select(x => x.Date ?? x.OriginalDate).DefaultIfEmpty().MaxAsync();

            HasBudgets = await db.Budgets.ByClient(currentClient).AnyAsync();

            return this;
        }

        public bool HasLines { get; set; }
        public bool HasBudgets { get; set; }
        public DateTime CurrentDate { get; set; }
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }
    }
}
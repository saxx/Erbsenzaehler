using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Erbsenzaehler.Models;

namespace Erbsenzaehler.ViewModels.Today
{
    public class IndexViewModel
    {
        public async Task<IndexViewModel> Fill(Db db, Client currentClient, Month month)
        {
            CurrentDate = month.Date;

            var linesQuery = db.Lines.ByClient(currentClient);
            var linesQueryForMonth = linesQuery.ByMonth(month);

            HasLines = await linesQueryForMonth.AnyAsync();

            MinDate = await linesQuery.Select(x => x.Date ?? x.OriginalDate).DefaultIfEmpty().MinAsync();
            MaxDate = await linesQuery.Select(x => x.Date ?? x.OriginalDate).DefaultIfEmpty().MaxAsync();

            HasBudgets = await db.Budgets.ByClient(currentClient).AnyAsync();

            Balance = await linesQueryForMonth.ByNotIgnored().Select(x => x.Amount ?? x.OriginalAmount).DefaultIfEmpty().SumAsync();

            return this;
        }


        public bool HasLines { get; set; }
        public bool HasBudgets { get; set; }
        public DateTime CurrentDate { get; set; }
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }
        public decimal Balance { get; set; }
    }
}
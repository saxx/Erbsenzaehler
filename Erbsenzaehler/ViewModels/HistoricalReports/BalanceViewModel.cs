using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Erbsenzaehler.Models;
using Erbsenzaehler.Reporting;

namespace Erbsenzaehler.ViewModels.HistoricalReports
{
    public class BalanceViewModel
    {
        public async Task<BalanceViewModel> Fill(Db db, Client client)
        {
            var amounts = await (from x in db.Lines.Where(x => x.Account.ClientId == client.Id && !x.Ignore)
                group x by new { (x.Date ?? x.OriginalDate).Year, (x.Date ?? x.OriginalDate).Month }
                into g
                orderby g.Key.Year, g.Key.Month
                select new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Balance = g.Select(y => (y.Amount ?? y.OriginalAmount)).DefaultIfEmpty(0).Sum()
                }).ToListAsync();

            BalancePerMonth = new Dictionary<Date, decimal>();
            foreach (var amount in amounts)
            {
                var date = new Date(amount.Year, amount.Month);
                BalancePerMonth[date] = amount.Balance;
            }

            return this;
        }


        public IDictionary<Date, decimal> BalancePerMonth { get; set; }
    }
}
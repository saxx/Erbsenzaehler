using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Erbsenzaehler.Models;
using Erbsenzaehler.Reporting;

namespace Erbsenzaehler.ViewModels.Reports
{
    public class BudgetViewModel
    {

        public async Task<BudgetViewModel> Fill(Db db, Client currentClient, int year, int month)
        {
            MinDate = db.Lines.Where(x => x.Account.ClientId == currentClient.Id).Select(x => x.Date ?? x.OriginalDate).DefaultIfEmpty().Min();
            MinDate = new DateTime(MinDate.Year, MinDate.Month, 1);
            MaxDate = db.Lines.Where(x => x.Account.ClientId == currentClient.Id).Select(x => x.Date ?? x.OriginalDate).DefaultIfEmpty().Max();
            MaxDate = new DateTime(MaxDate.Year, MaxDate.Month, 1).AddMonths(1).AddDays(-1);

            var startDate = new DateTime(year, month, 1);
            if (startDate < MinDate)
            {
                startDate = MinDate;
            }
            if (startDate > MaxDate)
            {
                startDate = MaxDate;
            }
            var endDate = startDate.AddMonths(1).AddDays(-1);

            Calculator = new BudgetCalculator(db, currentClient);
            await Calculator.Calculate(startDate, endDate);
            return this;
        }

        public BudgetCalculator Calculator { get; set; }
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }
    }
}
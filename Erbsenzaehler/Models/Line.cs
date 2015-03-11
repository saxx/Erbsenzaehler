using System;
using System.Linq;

namespace Erbsenzaehler.Models
{
    public static class LineExtensionMethods
    {
        public static IQueryable<Line> ByMonth(this IQueryable<Line> q, DateTime date)
        {
            var startDate = new DateTime(date.Year, date.Month, 1);
            var endDate = startDate.AddMonths(1).AddSeconds(-1);

            return q.Where(x => (x.Date ?? x.OriginalDate) >= date && (x.Date ?? x.OriginalDate) <= endDate);
        }

        public static IQueryable<Line> ByMonth(this IQueryable<Line> q, Month month)
        {
            return ByMonth(q, month.Date);
        }

        public static IQueryable<Line> ByClient(this IQueryable<Line> q, int clientId)
        {
            return q.Where(x => x.Account.ClientId == clientId);
        }

        public static IQueryable<Line> ByClient(this IQueryable<Line> q, Client client)
        {
            return q.Where(x => x.Account.ClientId == client.Id);
        }
    }

    public class Line
    {
        public int Id { get; set; }
        public int AccountId { get; set; }

        public decimal OriginalAmount { get; set; }
        public DateTime OriginalDate { get; set; }
        public string OriginalText { get; set; }

        public string Category { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Amount { get; set; }

        public bool Ignore { get; set; }
        public bool IgnoreUpdatedManually { get; set; }
        public bool CategoryUpdatedManually { get; set; }
        public bool DateUpdatedManually { get; set; }
        public bool AmountUpdatedManually { get; set; }

        public virtual Account Account { get; set; }
    }
}
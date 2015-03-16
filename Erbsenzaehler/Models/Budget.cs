using System;
using System.Linq;

namespace Erbsenzaehler.Models
{
    public static class BudgetExtensionMethods
    {
        public static IQueryable<Budget> ByClient(this IQueryable<Budget> q, int clientId)
        {
            return q.Where(x => x.ClientId == clientId);
        }

        public static IQueryable<Budget> ByClient(this IQueryable<Budget> q, Client client)
        {
            return q.Where(x => x.ClientId == client.Id);
        }
    }


    public class Budget
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }


        public string Category { get; set; }
        public decimal Limit { get; set; }
        public LimitPeriod Period { get; set; }


        public enum LimitPeriod
        {
            Weekly,
            Monthly,
            Yearly,
            Daily
        }

        public decimal NormalizeLimit(Month month)
        {
            switch (Period)
            {
                case LimitPeriod.Weekly:
                    // ReSharper disable once PossibleLossOfFraction
                    return Limit * (DateTime.DaysInMonth(month.Date.Year, month.Date.Month) / 7);
                case LimitPeriod.Monthly:
                    return Limit;
                case LimitPeriod.Yearly:
                    return Limit / 12;
                case LimitPeriod.Daily:
                    return Limit * DateTime.DaysInMonth(month.Date.Year, month.Date.Month);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
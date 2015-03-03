using System;

namespace Erbsenzaehler.Models
{
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

        public decimal LimitInDays
        {
            get
            {
                switch (Period)
                {
                    case LimitPeriod.Weekly:
                        return Limit / 7;
                    case LimitPeriod.Monthly:
                        return Limit / 30;
                    case LimitPeriod.Yearly:
                        return Limit / 365;
                    case LimitPeriod.Daily:
                        return Limit;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
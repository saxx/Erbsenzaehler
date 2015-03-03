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
            Yearly
        }
    }
}
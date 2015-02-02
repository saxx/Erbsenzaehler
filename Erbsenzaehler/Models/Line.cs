using System;

namespace Erbsenzaehler.Models
{
    public class Line
    {
        public int Id { get; set; }
        public int AccountId { get; set; }

        public double OriginalAmount { get; set; }
        public DateTime OriginalDate { get; set; }
        public string OriginalText { get; set; }

        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public bool Ignore { get; set; }
        public bool UpdatedManually { get; set; }

        public virtual Account Account { get; set; }
    }
}
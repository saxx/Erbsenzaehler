using System;

namespace Erbsenzaehler.Models
{
    public class Line
    {
        public int Id { get; set; }
        public int AccountId { get; set; }

        public decimal OriginalAmount { get; set; }
        public DateTime OriginalDate { get; set; }
        public string OriginalText { get; set; }

        public string Category { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public bool Ignore { get; set; }
        public bool UpdatedManually { get; set; }

        public virtual Account Account { get; set; }
    }
}
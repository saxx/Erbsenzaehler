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
        public DateTime? Date { get; set; }
        
        public bool Ignore { get; set; }
        public bool IgnoreUpdatedManually { get; set; }
        public bool CategoryUpdatedManually { get; set; }
        public bool DateUpdatedManually { get; set; }

        public virtual Account Account { get; set; }
    }
}
using System;
namespace Erbsenzaehler.Models
{
    public class SummaryMailLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public virtual User User { get; set; }

    }
}

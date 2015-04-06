using System;
using System.Linq;

namespace Erbsenzaehler.Models
{
    public static class SummaryMailLogExtensions
    {
        public static IQueryable<SummaryMailLog> ByUser(this IQueryable<SummaryMailLog> query, User user)
        {
            return query.Where(x => x.UserId == user.Id);
        } 
    }

    public class SummaryMailLog
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime Date { get; set; }
        public virtual User User { get; set; }

    }
}

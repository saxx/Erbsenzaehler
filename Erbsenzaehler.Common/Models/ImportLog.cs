using System;
using System.Linq;

namespace Erbsenzaehler.Models
{
    public static class ImportLogExtensions
    {
        public static IQueryable<ImportLog> ByClient(this IQueryable<ImportLog> query, Client client)
        {
            return query.Where(x => x.Account.ClientId == client.Id);
        }


        public static IQueryable<ImportLog> ByAccount(this IQueryable<ImportLog> query, Account account)
        {
            return query.Where(x => x.AccountId == account.Id);
        }


        public static IQueryable<ImportLog> ByUser(this IQueryable<ImportLog> query, User user)
        {
            return query.Where(x => x.UserId == user.Id);
        }


        public static IQueryable<ImportLog> ByNoError(this IQueryable<ImportLog> query)
        {
            return query.Where(x => x.LinesFoundCount > 0);
        }
    }

    public class ImportLog
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string UserId { get; set; }

        public DateTime Date { get; set; }

        public int LinesFoundCount { get; set; }
        public int LinesImportedCount { get; set; }
        public int LinesDuplicatesCount { get; set; }

        public int Milliseconds { get; set; }
        public string Log { get; set; }
        public ImportLogType Type { get; set; }

        public virtual Account Account { get; set; }
        public virtual User User { get; set; }
    }
}
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Erbsenzaehler.Models;

namespace Erbsenzaehler.ViewModels.ManageAccounts
{
    public class IndexViewModel
    {

        public async Task<IndexViewModel> Fill(Db db, Client currentClient)
        {
            Accounts = await db.Accounts
                .Where(x => x.ClientId == currentClient.Id)
                .OrderBy(x => x.Name)
                .Select(x => new Account
                {
                    Id = x.Id,
                    LinesCount = x.Lines.Count,
                    Name = x.Name
                })
                .ToListAsync();

            return this;
        }


        public IEnumerable<Account> Accounts { get; set; }


        public class Account
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public int LinesCount { get; set; }
        }

    }
}
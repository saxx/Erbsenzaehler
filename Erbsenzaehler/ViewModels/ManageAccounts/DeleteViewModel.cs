using System.Data.Entity;
using System.Threading.Tasks;
using Erbsenzaehler.Models;

namespace Erbsenzaehler.ViewModels.ManageAccounts
{
    public class DeleteViewModel
    {
        public async Task<DeleteViewModel> Fill(Db db, Client currentClient, Models.Account account)
        {
            Account = account;
            NumberOfLines = await db.Lines.CountAsync(x => x.AccountId == account.Id);
            IsLastAccount = await db.Accounts.CountAsync(x => x.ClientId == currentClient.Id) <= 1;

            return this;
        }


        public Models.Account Account { get; set; }
        public int NumberOfLines { get; set; }
        public bool IsLastAccount { get; set; }
    }
}
using System.Data.Entity;
using System.Threading.Tasks;
using Erbsenzaehler.Models;

namespace Erbsenzaehler.ViewModels.Lines
{
    public class IndexViewModel
    {
        public async Task<IndexViewModel> Fill(Db db, Client currentClient)
        {
            HasLines = await db.Lines.AnyAsync(x => x.Account.ClientId == currentClient.Id);

            return this;
        }


        public bool HasLines { get; set; }
    }
}
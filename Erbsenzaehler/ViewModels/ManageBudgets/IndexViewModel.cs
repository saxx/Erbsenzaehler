using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Erbsenzaehler.Models;

namespace Erbsenzaehler.ViewModels.ManageBudgets
{
    public class IndexViewModel
    {
        public async Task<IndexViewModel> Fill(Db db, Client currentClient)
        {
            var categories = await db.Lines
                .Where(x => x.Account.ClientId == currentClient.Id)
                .Select(x => x.Category)
                .Distinct()
                .Where(x => x != null && x != "")
                .ToListAsync();

            categories.AddRange(await db.Rules
                .Where(x => x.ClientId == currentClient.Id)
                .Select(x => x.ChangeCategoryTo)
                .Distinct()
                .Where(x => x != null && x != "")
                .ToListAsync()
                );

            Categories = categories.Distinct().ToList().OrderBy(x => x).ToList();

            return this;
        }


        public IEnumerable<string> Categories { get; set; }
    }
}
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Erbsenzaehler.Models;

namespace Erbsenzaehler.ViewModels.ManageUsers
{
    public class IndexViewModel
    {
        public async Task<IndexViewModel> Fill(Db db, Client currentClient)
        {
            Users = await db.Users
                .Where(x => x.ClientId == currentClient.Id)
                .OrderBy(x => x.Email)
                .Select(x => new User
                {
                    Email = x.Email,
                    Id = x.Id
                }).ToListAsync();
            return this;
        }


        public IEnumerable<User> Users { get; set; }

        public class User
        {
            public string Id { get; set; }
            public string Email { get; set; }
        }
    }
}
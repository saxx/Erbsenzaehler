using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Erbsenzaehler.Models
{
    public class Db : IdentityDbContext<User>
    {
        public Db() : base("Db")
        {
            //Database.SetInitializer(new DropCreateDatabaseAlways<Db>());
        }


        public static Db Create()
        {
            return new Db();
        }


        public DbSet<Client> Clients { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Line> Lines { get; set; }
    }
}
using System.Data.Entity;
using Erbsenzaehler.Migrations;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Erbsenzaehler.Models
{
    public class Db : IdentityDbContext<User>
    {
        public Db() : base("Db")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<Db, Configuration>());
        }


        public static Db Create()
        {
            return new Db();
        }


        public DbSet<Client> Clients { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Line> Lines { get; set; }
        public DbSet<Rule> Rules { get; set; }
        public DbSet<Budget> Budgets { get; set; }
    }
}
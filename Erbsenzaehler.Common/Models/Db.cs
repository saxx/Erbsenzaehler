using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Erbsenzaehler.Migrations;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Erbsenzaehler.Models
{
    public class Db : IdentityDbContext<User>
    {
        public Db(string connectionString) : base(connectionString)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<Db, Configuration>());
        }


        public static Db Create()
        {
            return new Db(Config.DatabaseConnectionString);
        }


        public DbSet<Client> Clients { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Line> Lines { get; set; }
        public DbSet<Rule> Rules { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<SummaryMailLog> SummaryMailLogs { get; set; }
        public DbSet<ImportLog> ImportLog { get; set; }
    }

    public class MyContextFactory : IDbContextFactory<Db>
    {
        public Db Create()
        {
            return new Db(Config.DatabaseConnectionString);
        }
    }
}
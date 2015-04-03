using System.Data.Entity.Migrations;
using Erbsenzaehler.Models;

namespace Erbsenzaehler.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<Db>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }


        protected override void Seed(Db context)
        {
        }
    }
}
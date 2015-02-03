using System;
using System.Data.Entity.Migrations;
using System.Linq;
using Erbsenzaehler.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

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
            context.Clients.AddOrUpdate(
                x => x.Id,
                new Client { Id = 1, Name = "Client #1" }
                );

            context.Accounts.AddOrUpdate(
                x => x.Id,
                new Account { Id = 1, Name = "Account #1", ClientId = 1 }
                );

            context.Rules.AddOrUpdate(
                x => x.Id,
                new Rule { Id = 1, ClientId = 1, ChangeCategoryTo = "Supermarkt", Regex = "Billa" }
                );

            context.Rules.AddOrUpdate(
                x => x.Id,
                new Rule { Id = 2, ClientId = 1, ChangeCategoryTo = "Supermarkt", Regex = "Merkur" }
                );

            context.Rules.AddOrUpdate(
                x => x.Id,
                new Rule { Id = 3, ClientId = 1, ChangeCategoryTo = "Drogerie", Regex = "Bipa" }
                );

            if (!context.Users.Any())
            {
                var userManager = new ApplicationUserManager(new UserStore<User>(context));

                var user = new User { UserName = "hannes@sachsenhofer.com", Email = "hannes@sachsenhofer.com", ClientId = 1 };
                var userResult = userManager.Create(user, "asdf.Qwer1");

                if (!userResult.Succeeded)
                {
                    throw new Exception("Unable to create seed user: " + string.Join("\n", userResult.Errors));
                }
            }
        }
    }
}
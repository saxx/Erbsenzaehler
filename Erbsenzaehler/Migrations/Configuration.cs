using Erbsenzaehler.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Erbsenzaehler.Migrations
{
	using System;
	using System.Data.Entity;
	using System.Data.Entity.Migrations;
	using System.Linq;

	internal sealed class Configuration : DbMigrationsConfiguration<Erbsenzaehler.Models.Db>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = false;
		}

		protected override void Seed(Erbsenzaehler.Models.Db context)
		{
			context.Clients.AddOrUpdate(
				x => x.Id,
				new Client { Id = 1, Name = "Client #1" }
			);

			context.Accounts.AddOrUpdate(
				x => x.Id,
				new Account { Id = 1, Name = "Account #1", ClientId = 1 }
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

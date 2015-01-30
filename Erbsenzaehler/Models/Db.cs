using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Erbsenzaehler.Models
{
	public class Db : IdentityDbContext<User>
	{
		public static Db Create()
		{
			return new Db();
		}

		public DbSet<Client> Clients { get; set; }
		public DbSet<Account> Accounts { get; set; }
		public DbSet<Line> Lines { get; set; }

	}
}
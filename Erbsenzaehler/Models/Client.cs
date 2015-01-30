using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Erbsenzaehler.Models
{
	public class Client
	{
		public int Id { get; set; }

		public virtual ICollection<User> Users { get; set; }
		public virtual ICollection<Account> Accounts { get; set; }

	}
}
using System.Collections.Generic;

namespace Erbsenzaehler.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string AutoImporterSettings { get; set; }
        

        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Rule> Rules { get; set; }
        public virtual ICollection<Budget> Budgets { get; set; }
    }
}
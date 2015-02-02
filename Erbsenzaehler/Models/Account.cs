using System.Collections.Generic;

namespace Erbsenzaehler.Models
{
    public class Account
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Line> Lines { get; set; }
    }
}
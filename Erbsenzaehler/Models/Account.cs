using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Erbsenzaehler.Models
{
    public class Account
    {
        public int Id { get; set; }
        public int ClientId { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<Line> Lines { get; set; }
    }
}
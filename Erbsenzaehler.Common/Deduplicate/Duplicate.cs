using System.Collections.Generic;
using Erbsenzaehler.Models;

namespace Erbsenzaehler.Deduplicate
{
    public class Duplicate
    {
        public Line Original { get; set; }
        public IEnumerable<Line> Duplicates { get; set; }
    }
}

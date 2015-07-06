using System.Collections.Generic;
using System.Linq;
using Erbsenzaehler.Deduplicate;
using Erbsenzaehler.Models;

namespace Erbsenzaehler.ViewModels.ManageAccounts
{
    public class DuplicatesViewModel
    {
        public DuplicatesViewModel Fill(Account account, IEnumerable<Duplicate> duplicates, bool useFuzzyMatching)
        {
            AccountId = account.Id;
            AccountName = account.Name;
            Duplicates = duplicates.OrderByDescending(x => x.Original.Date ?? x.Original.OriginalDate);
            UseFuzzyMatching = useFuzzyMatching;
            return this;
        }

        public IEnumerable<Duplicate> Duplicates { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public bool UseFuzzyMatching { get; set; }
    }
}
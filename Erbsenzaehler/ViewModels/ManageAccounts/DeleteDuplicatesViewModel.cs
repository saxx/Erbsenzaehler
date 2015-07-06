using Erbsenzaehler.Models;

namespace Erbsenzaehler.ViewModels.ManageAccounts
{
    public class DeleteDuplicatesViewModel
    {
        public DeleteDuplicatesViewModel Fill(Account account, bool usedFuzzyMatching)
        {
            Account = account;
            UsedFuzzyMatching = usedFuzzyMatching;
            return this;
        }


        public bool UsedFuzzyMatching { get; set; }
        public Account Account { get; set; }
    }
}
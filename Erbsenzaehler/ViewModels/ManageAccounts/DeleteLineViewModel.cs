using Erbsenzaehler.Models;

namespace Erbsenzaehler.ViewModels.ManageAccounts
{
    public class DeleteLineViewModel
    {
        public DeleteLineViewModel Fill(Line line)
        {
            Date = (line.Date ?? line.OriginalDate).ToShortDateString();
            Amount = (line.Amount ?? line.OriginalAmount).ToString("N2");
            Text = line.Text ?? line.OriginalText;
            return this;
        }


        public string Date { get; set; }
        public string Amount { get; set; }
        public string Text { get; set; }      
    }
}
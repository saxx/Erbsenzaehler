using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Erbsenzaehler.Models;

namespace Erbsenzaehler.ViewModels.ManageClient
{
    public class IndexViewModel
    {
        public IndexViewModel Fill(Client currentClient)
        {
            ClientName = currentClient.Name;

            return this;
        }


        [Required(ErrorMessage = "Your clients name must not be empty.")]
        [DisplayName("Client name")]
        public string ClientName { get; set; }

        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
    }
}
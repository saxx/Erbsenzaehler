using System.ComponentModel.DataAnnotations;
using Erbsenzaehler.Models;

namespace Erbsenzaehler.ViewModels.ManageUser
{
    public class IndexViewModel
    {
        public IndexViewModel Fill(Models.User user)
        {
            UserName = user.UserName;
            Email = user.Email;
            SummaryMailInterval = user.SummaryMailInterval;

            return this;
        }


        [Display(Name = "Ihr Benutzername")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie Ihre E-Mail-Adresse an.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Ihre E-Mail-Adresse")]
        public string Email { get; set; }

        [Display(Name = "Zusammenfassung per E-Mail")]
        public SummaryMailIntervalOptions SummaryMailInterval { get; set; }
    }
}
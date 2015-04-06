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


        [Display(Name = "Your username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Enter your e-mail address.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Your e-mail address")]
        public string Email { get; set; }

        [Display(Name = "Summary mail")]
        public SummaryMailIntervalOptions SummaryMailInterval { get; set; }
    }
}
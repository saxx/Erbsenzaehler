using System.ComponentModel.DataAnnotations;

namespace Erbsenzaehler.ViewModels.ManageUsers
{
    public class CreateViewModel
    {
        [Required(ErrorMessage = "Bitte geben Sie eine E-Mail-Adfesse an.")]
        [EmailAddress]
        [Display(Name = "E-Mail-Adresse")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Bitte wählen Sie ein Passwort.")]
        [StringLength(100, ErrorMessage = "Das Passwort muss mindestens {2} Zeichen lang sein.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Passwort wählen")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Passwort bestätigen")]
        [Compare("Password", ErrorMessage = "Die angegebenen Passwörter stimmen nicht überein.")]
        public string ConfirmPassword { get; set; }
    }
}
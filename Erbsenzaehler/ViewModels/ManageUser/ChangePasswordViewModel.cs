using System.ComponentModel.DataAnnotations;

namespace Erbsenzaehler.ViewModels.ManageUser
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Bitte geben Sie Ihr derzeitiges Passwort an.")]
        [DataType(DataType.Password)]
        [Display(Name = "Ihr derzeitiges Passwort")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Bitte wählen Sie ein neues Passwort.")]
        [StringLength(100, ErrorMessage = "Das Passwort muss mindestens {2} Zeichen lang sein.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Neues Passwort wählen")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Neues Passwort bestätigen")]
        [Compare("NewPassword", ErrorMessage = "Die beiden neuen Passwörter stimmen nicht überein.")]
        public string ConfirmPassword { get; set; }

        public bool WasSuccessful { get; set; }
    }
}
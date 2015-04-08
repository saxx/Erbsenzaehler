using System.ComponentModel.DataAnnotations;

namespace Erbsenzaehler.ViewModels.User
{
    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Bitte geben Sie Ihre E-Mail-Adresse an.")]
        [Display(Name = "Ihre E-Mail-Adresse")]
        [EmailAddress(ErrorMessage = "Bitte geben Sie Ihre E-Mail-Adresse an.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie Ihr Passwort an.")]
        [DataType(DataType.Password)]
        [Display(Name = "Ihr Passwort")]
        public string Password { get; set; }

        [Display(Name = "Anmeldung merken?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Bitte geben Sie Ihre E-Mail-Adresse an.")]
        [EmailAddress]
        [Display(Name = "Ihre E-Mail-Adresse")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Bitte wählen Sie ein Passwort.")]
        [StringLength(100, ErrorMessage = "Das Passwort muss mindestes {2} Zeichen lang sein.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Passwort wählen")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Passwort bestätigen")]
        [Compare("Password", ErrorMessage = "Die angegebenen Passwörter stimmen nicht überein.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-Mail-Adresse")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Das Passwprt muss mindestens {2} Zeiche lang sein.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Passwort wählen")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Passwort bestätigen")]
        [Compare("Password", ErrorMessage = "Die angegebenen Passwörter stimmen nicht überein.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-Mail-Adresse")]
        public string Email { get; set; }
    }
}
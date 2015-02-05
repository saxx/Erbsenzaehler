using System.ComponentModel.DataAnnotations;

namespace Erbsenzaehler.ViewModels.ManageUser
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Enter your current password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Your current password")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Choose a new password.")]
        [StringLength(100, ErrorMessage = "The password must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Choose new password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public bool WasSuccessful { get; set; }
    }
}
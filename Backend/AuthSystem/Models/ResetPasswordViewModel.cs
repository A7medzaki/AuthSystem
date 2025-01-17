using System.ComponentModel.DataAnnotations;

namespace AuthSystem.Models
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Password Is Required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{6,}$", ErrorMessage = "Password must be at least 6 characters long, contain at least one lowercase letter, one uppercase letter, one digit, and one non-alphanumeric character.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password Is Required")]
        [Compare(nameof(Password), ErrorMessage = "Confirm Password does not match Password")]
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}

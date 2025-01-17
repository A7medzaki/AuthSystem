using System.ComponentModel.DataAnnotations;

namespace AuthSystem.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Format For Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password Is Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}

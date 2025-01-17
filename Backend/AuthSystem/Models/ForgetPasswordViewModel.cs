using System.ComponentModel.DataAnnotations;

namespace AuthSystem.Models
{
    public class ForgetPasswordViewModel
    {
        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Format For Email")]
        public string Email { get; set; }
    }
}

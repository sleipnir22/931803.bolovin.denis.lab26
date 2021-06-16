using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required] [EmailAddress]
        public string Email { get; set; }

        [Required] 
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required] 
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
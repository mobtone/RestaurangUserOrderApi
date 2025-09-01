using System.ComponentModel.DataAnnotations;

namespace AzureAppPizzeria.Data.Dtos
{
    public class ChangePasswordDto
    {

        [Required]
        public string? CurrentPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)] //Matchar övriga lösenordsregler
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}
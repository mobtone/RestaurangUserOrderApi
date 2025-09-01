using System.ComponentModel.DataAnnotations;

namespace AzureAppPizzeria.Data.Dtos.Authorization
{
    public class RegisterUserDto
    {
        [Required]
        [StringLength(50, ErrorMessage = "Username must be at most {1} characters long.")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        public string? Password { get; set; }

        public string? PhoneNumber { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace AzureAppPizzeria.Data.Dtos
{
    public class UserUpdateDto
    {
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(256)] //max längden för email i identity core
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string? PhoneNumber { get; set; }
    }
}

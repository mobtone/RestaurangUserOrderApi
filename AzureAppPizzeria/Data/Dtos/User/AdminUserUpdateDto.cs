using System.ComponentModel.DataAnnotations;

namespace AzureAppPizzeria.Data.Dtos.User
{
    public class AdminUserUpdateDto
    {
        [Required]
        public string? NewRole { get; set; }
    }
}

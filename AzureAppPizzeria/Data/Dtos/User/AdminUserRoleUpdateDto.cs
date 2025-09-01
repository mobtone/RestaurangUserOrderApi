using System.ComponentModel.DataAnnotations;

namespace AzureAppPizzeria.Data.Dtos.User
{
    public class AdminUserRoleUpdateDto
    {
        [Required(ErrorMessage = "New role is required.")]
        public string? NewRole { get; set; } //"PremiumUser" eller "RegularUser"
    }
}

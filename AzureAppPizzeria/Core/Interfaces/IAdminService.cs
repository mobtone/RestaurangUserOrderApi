using AzureAppPizzeria.Data.Dtos.User;

namespace AzureAppPizzeria.Core.Interfaces
{
    public interface IAdminService
    {
        Task<UserResponseDto?> GetUserByAdmin(string searchIdentifier);

    }
}

using AzureAppPizzeria.Data.Dtos.Authorization;
using Microsoft.AspNetCore.Identity;

namespace AzureAppPizzeria.Core.Interfaces
{
    public interface IAuthService
    {
        Task<(IdentityResult result, string? userId)> RegisterUser(RegisterUserDto registerDto);
        Task<(bool success, AuthResponseDto? authResponse)> LoginUser(LoginDto loginDto);
    }
}

using AzureAppPizzeria.Data.Dtos;
using AzureAppPizzeria.Data.Dtos.User;
using AzureAppPizzeria.Data.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AzureAppPizzeria.Core.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto?> GetLoggedInUser(string userId);
        Task<List<UserResponseDto>> GetAllUsers();
        Task<IdentityResult> UpdateUser(string userId, UserUpdateDto updateDto); //Email och password ska gå att uppdatera, enligt UserUpdateDto
        Task<IdentityResult> ChangePassword(string userId, ChangePasswordDto changePasswordDto); //metod för att ändra lösenord
        Task<IdentityResult> ChangeUserRole(string userId, string newRole); //för att admin ska kunna ändra roll på användare
    }
}

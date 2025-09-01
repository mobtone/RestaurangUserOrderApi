using AzureAppPizzeria.Core.Interfaces;
using AzureAppPizzeria.Data.Dtos;
using AzureAppPizzeria.Data.Dtos.User;
using AzureAppPizzeria.Data.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AzureAppPizzeria.Core.Services
{
    //Denna klass ska använda sig av interface Bonusservice också 
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<UserService> _logger;
        public UserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
            ILogger<UserService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<IdentityResult> ChangePassword(string userId, ChangePasswordDto changePasswordDto)
        { 
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", userId);
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = "User not found"
                });
            }
            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            if (!result.Succeeded)
            {
                _logger.LogWarning("User {UserId} failed to change password. Errors: {Errors}", userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                return result;
            }
            else
            {
                _logger.LogInformation("Password changed for user {UserId}", userId);
            }

            return result;
        }

        public async Task<IdentityResult> ChangeUserRole(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", userId);
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = "User not found"
                });
            }
            //ta bort användarens nuvarande roll
            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                _logger.LogWarning("Failed to remove roles for user {UserId}. Errors: {Errors}", userId, string.Join(", ", removeResult.Errors.Select(e => e.Description)));
                return removeResult;
            }
            //ny roll för användaren
            if (!await _roleManager.RoleExistsAsync(newRole))
            {
                _logger.LogWarning("Role {NewRole} does not exist", newRole);
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "RoleNotFound",
                    Description = "Role not found"
                });
            }
            var addResult = await _userManager.AddToRoleAsync(user, newRole);
            if (!addResult.Succeeded)
            {
                _logger.LogWarning("Failed to add role {NewRole} to user {UserId}. Errors: {Errors}", newRole, userId, string.Join(", ", addResult.Errors.Select(e => e.Description)));
                return addResult;
            }

            if (currentRoles.Contains("PremiumUser") && newRole == "RegularUser")
            {
                if (user.BonusPoints > 0) // Bara om det faktiskt finns poäng att nollställa
                {
                    _logger.LogInformation(
                        "User {UserId} downgraded from PremiumUser to RegularUser. Resetting BonusPoints from {OldBonusPoints} to 0.",
                        userId, user.BonusPoints);
                    user.BonusPoints = 0;
                    var updateResult = await _userManager.UpdateAsync(user); // Spara ändringen av användarobjektet
                    if (!updateResult.Succeeded)
                    {
                        _logger.LogError(
                            "Failed to reset bonus points for user {UserId} after role change to RegularUser. Errors: {Errors}",
                            userId, string.Join(", ", updateResult.Errors.Select(e => e.Description)));
                       
                    }
                }
            }
            _logger.LogInformation("User {UserId} role changed to {NewRole}", userId, newRole);
            return IdentityResult.Success;
        }

        public async Task<List<UserResponseDto>> GetAllUsers()
        {
            _logger.LogInformation("Admin attempting to fetch all users with their order IDs.");

            var users = await _userManager.Users
                .Include(u => u.Orders) 
                .ToListAsync();

            var userResponseDtos = new List<UserResponseDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userResponseDtos.Add(new UserResponseDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    BonusPoints = user.BonusPoints,
                    Roles = roles.ToList(), 
                    OrderIds = user.Orders?.Select(o => o.OrderId).ToList() ?? new List<int>()
                });
            }

            _logger.LogInformation("Successfully fetched {UserCount} users.", userResponseDtos.Count);
            return userResponseDtos; 
        }

        public async Task<UserResponseDto?> GetLoggedInUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", userId);
                return null;
            }
            var roles = await _userManager.GetRolesAsync(user);
            return new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                BonusPoints = user.BonusPoints,
                Roles = roles.ToList()
            };
        }
        public async Task<IdentityResult> UpdateUser(string userId, UserUpdateDto updateDto)
        {
            var userClaims = new ClaimsPrincipal();
            var userResponse = await GetLoggedInUser(userId);

            //Användaren hämtas först med hjälp av GetUserById metoden för att se om den inloggade användaren finns i databasen med matchande ID
            ////Om den inte finns så loggas en varning och en IdentityResult med felmeddelande returneras
            //Detta görs som en säkerhetsåtgärd för att förhindra att en användare försöker uppdatera en annan användares information
            //Om användaren finns så hämtas den med hjälp av _userManager.FindByIdAsync metoden

            if (userResponse == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", userId);
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = "User not found"
                });
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", userId);
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = "User not found"
                });
            }
            IdentityResult result = IdentityResult.Success;

            if (!string.IsNullOrWhiteSpace(updateDto.Email) &&
                !user.Email.Equals(updateDto.Email, StringComparison.OrdinalIgnoreCase))
            {
                result = await _userManager.SetEmailAsync(user, updateDto.Email);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("User {UserId} failed to update email to {NewEmail}, Errors: {Errors}", userId, updateDto.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                    return result;
                }
            }
                     
            if (!string.IsNullOrWhiteSpace(updateDto.PhoneNumber) &&
                !user.PhoneNumber.Equals(updateDto.PhoneNumber, StringComparison.OrdinalIgnoreCase))
            {
                result = await _userManager.SetPhoneNumberAsync(user, updateDto.PhoneNumber);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("User {UserId} failed to update PhoneNumber. Errors: {Errors}", userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                    return result; 
                }
                _logger.LogInformation("User {UserId} PhoneNumber updated to {NewPhoneNumber}", userId, updateDto.PhoneNumber);
            }

            //Om jag kommer hit och 'result' fortfarande är IdentityResult.Success,
            //betyder det antingen att inga ändringar gjordes eller att alla ändringar lyckades.
            //de inbyggda metoderna SetEmailAsync, SetUserNameAsync, SetPhoneNumberAsync anropar UpdateAsync internt
            // så ett extra _userManager.UpdateAsync(user) behövs normalt inte då
            _logger.LogInformation("User {UserId} updated", userId);
            return result;
        }
    }
}

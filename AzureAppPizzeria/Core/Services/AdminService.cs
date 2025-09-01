using AzureAppPizzeria.Core.Interfaces;
using AzureAppPizzeria.Data.Dtos.User;
using AzureAppPizzeria.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace AzureAppPizzeria.Core.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<AdminService> _logger;

        public AdminService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
            ILogger<AdminService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<UserResponseDto?> GetUserByAdmin(string searchIdentifier)
        {
            var user = await _userManager.FindByNameAsync(searchIdentifier)
                       ?? await _userManager.FindByEmailAsync(searchIdentifier);
            if (user == null)
            {
                _logger.LogWarning("User with identifier {SearchIdentifier} not found", searchIdentifier);
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
    }
}


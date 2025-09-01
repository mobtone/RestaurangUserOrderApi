using System.Net.Mime;
using AzureAppPizzeria.Core.Interfaces;
using AzureAppPizzeria.Data.Configurations;
using AzureAppPizzeria.Data.Entities;
using AzureAppPizzeria.Data;
using AzureAppPizzeria.Data.Dtos.Authorization;
using AzureAppPizzeria.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AzureAppPizzeria.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthService> _logger;

        public AuthService(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager, JwtSettings jwtSettings, ILogger<AuthService> logger, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtSettings = jwtSettings;
            _logger = logger;
            _signInManager = signInManager;
        }
        //RegisterUser returnerar en IdentityResult och en string som är userId
        //IdentityResult är en klass som innehåller information om resultatet av
        //en operation tex om den lyckades eller misslyckades
        //userId är en sträng som representerar användarens unika ID i databasen
        //RegisterUserDto är en klass som innehåller information om användaren som
        // ska registreras, tex användarnamn, lösenord, email osv
        //defaultRole är en sträng som representerar den roll som användaren ska
        //tilldelas vid registreringen, detta hanteras av Core Identity

        public async Task<(IdentityResult, string)> RegisterUser(RegisterUserDto registerUserDto)
        {
            var existingUsername = await _userManager.FindByNameAsync(registerUserDto.Username!);
            if (existingUsername != null)
            {
                _logger.LogWarning("Username {Username} is already taken.", registerUserDto.Username);
                var usernameError = IdentityResult.Failed(new IdentityError
                {
                    Code = "UsernameTaken",
                    Description = "Username is already taken."
                });
                return (usernameError, null);
            }
            var existingEmail = await _userManager.FindByEmailAsync(registerUserDto.Email!);
            if (existingEmail != null)
            {
                _logger.LogWarning("Email {Email} is already registered.", registerUserDto.Email);
                var emailError = IdentityResult.Failed(new IdentityError
                {
                    Code = "EmailTaken",
                    Description = "Email is already registered."
                });
                return (emailError, null);
            }

            var user = new ApplicationUser
            {
                UserName = registerUserDto.Username,
                Email = registerUserDto.Email,
                PhoneNumber = registerUserDto.PhoneNumber,
                BonusPoints = 0
            };

            var result = await _userManager.CreateAsync(user, registerUserDto.Password);
            if (!result.Succeeded)
            {
                _logger.LogWarning("User registration failed: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
                return (result, null);
            }
            _logger.LogInformation("User {Email} created successfully with Id {UserId}", user.UserName, user.Email, user.Id);

            //Här tilldelas rollen RegularUser vid registrering
            const string defaultRole = "RegularUser";

            if (!await _roleManager.RoleExistsAsync(defaultRole))
            {
                //Om seedingen av roller fungerar korrekta så ska denna rad inte köras
                _logger.LogError("Warning: Default role '{DefaultRole}' does not exist, cannot be assigned to user {Email}");
                var roleMissingError = IdentityResult.Failed(new IdentityError
                {
                    Code = "RoleMissing",
                    Description = $"Default role '{defaultRole}' does not exist."
                });
                return (roleMissingError, user.Id);
            }
            //denna rad lägger till användaren i rollen RegularUser och returnerar
            //ett IdentityResult som innehåller information om resultatet av operationen
            var addRoleResult = await _userManager.AddToRoleAsync(user, defaultRole);
            if (!addRoleResult.Succeeded)
            {
                _logger.LogWarning("Failed to add user {Email} to role {DefaultRole}: {Errors}",
                    user.Email, defaultRole,
                    string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
                return (addRoleResult, user.Id);
            }
            _logger.LogInformation("User {Email} added to role {Role}", user.Email, defaultRole);
            return (IdentityResult.Success, user.Id);
        }

        public async Task<(bool success, AuthResponseDto? authResponse)> LoginUser(LoginDto loginDto)
        {
            var result = await _signInManager.PasswordSignInAsync(
                loginDto.LoginIdentifier,
                loginDto.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Login failed for identifier {LoginIdentifier}.", loginDto.LoginIdentifier);
                return (false, null);
            }

            var user = await _userManager.FindByNameAsync(loginDto.LoginIdentifier)
                       ?? await _userManager.FindByEmailAsync(loginDto.LoginIdentifier);

            if (user == null)
            {
                _logger.LogWarning("User not found after successful login for identifier {LoginIdentifier}.", loginDto.LoginIdentifier);
                return (false, null);
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var token = JwtAuthExtension.GenerateJwtToken(user, userRoles, _jwtSettings);
            var tokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInHours);

            _logger.LogInformation("User {UserName_Email} logged in successfully.", user.UserName ?? user.Email);

            return (true, new AuthResponseDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Token = token,
                TokenExpiration = tokenExpiration,
                Roles = userRoles.ToList()
            });
        }
    }
}
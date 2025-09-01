using AzureAppPizzeria.Core.Interfaces;
using AzureAppPizzeria.Data.Dtos.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureAppPizzeria.Controllers
{
    //controller som hanterar registering och inloggning. injicera services som behövs för 

    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthorizationController> _logger;

        public AuthorizationController(IAuthService authService, ILogger<AuthorizationController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (result, userId) = await _authService.RegisterUser(registerDto);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                _logger.LogWarning("Registration failed for {Email}: {Errors}", registerDto.Email,
                    string.Join(", ", result.Errors.Select(e => e.Description)));
                return BadRequest(ModelState);
            }

            _logger.LogInformation("User {Email} registered with Id {UserId}.", registerDto.Email, userId);
            return Ok(new { UserId = userId, Message = "User registered successfully." });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (success, authResponse) = await _authService.LoginUser(loginDto);
            if (!success)
            {
                _logger.LogWarning("Login failed for {LoginIdentifier}", loginDto.LoginIdentifier);
                return Unauthorized(new { Message = "Invalid login attempt." });
            }

            _logger.LogInformation("User {LoginIdentifier} logged in successfully.", loginDto.LoginIdentifier);
            return Ok(new
            {
                Token = authResponse?.Token,
                Expiration = authResponse?.TokenExpiration,
                Message = "Login successful"
            });
        }
        [Authorize]
        [HttpPost("Logout")]
        public async Task <IActionResult> Logout()
        {
            Response.Cookies.Delete(".AspNetCore.Identity.Application");
            return Ok(new { Message = "Logged out successfully." });
        }

    }
}
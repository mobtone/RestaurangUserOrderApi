using System.Runtime.InteropServices.JavaScript;
using AzureAppPizzeria.Core.Interfaces;
using AzureAppPizzeria.Data.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AzureAppPizzeria.Data.Dtos.User;

namespace AzureAppPizzeria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, IUserService service)
        {
            _logger = logger;
            _userService = service;
        }

        [HttpGet("Profile")]
        public async Task<IActionResult> GetLoggedInUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //hämtar id från den validerade JWTn
            if (string.IsNullOrEmpty(userId))
            { _logger.LogWarning("User ID claim (NameIdentifier/Sub) not found in token");
            }

            _logger.LogInformation("Fetching profile for user ID: {UserId}", userId);
            var userDto = await _userService.GetLoggedInUser(userId);

            if (userDto == null)
            {
                _logger.LogWarning("User profile not found for user ID: {UserId} during GetLoggedInUser", userId);
                return NotFound(new { Message = "Du måste vara inloggad för att använda denna funktion" });
            }
            return Ok(userDto);
        }

        [Authorize]
        [HttpPut("UpdateAccount")]
        public async Task<IActionResult> UpdateAccount([FromBody] UserUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID claim (NameIdentifier) not found in token for an authorized request to UpdateAccount.");
                return BadRequest(new { Message = "User ID not found" });
            }
            var result = await _userService.UpdateUser(userId, updateDto);
            if (!result.Succeeded)
            {
                return BadRequest(new { Message = "Failed to update account", Errors = result.Errors });
            }

            return Ok(new { Message = "Account updated successfully" });
        }
        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User Id claim not found in token");
                return BadRequest(new { Message = "User Id not found" });
            }
            var result = await _userService.ChangePassword(userId, changePasswordDto);
            if (!result.Succeeded)
            {
                _logger.LogWarning("User not found for Id {UserId} during ChangePassword", userId);
                return BadRequest(new { Message = "Failed to change password", Errors = result.Errors });
            }

            return Ok(new { Message = "Password changed successfully." });
        }
    }
}

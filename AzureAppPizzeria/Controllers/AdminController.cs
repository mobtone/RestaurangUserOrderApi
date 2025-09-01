using AzureAppPizzeria.Core.Interfaces;
using AzureAppPizzeria.Core.Services;
using AzureAppPizzeria.Data.Dtos.Meal;
using AzureAppPizzeria.Data.Dtos.Order;
using AzureAppPizzeria.Data.Dtos.User;
using AzureAppPizzeria.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureAppPizzeria.Controllers
{
    //Endpoints för att admin som styr användarhantering, orderhantering
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;
        private readonly IMealService _mealService;
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger, IMealService mealService,
            IOrderService orderService, IUserService userService)
        {
            _adminService = adminService;
            _logger = logger;
            _mealService = mealService;
            _orderService = orderService;
            _userService = userService;
        }

        [HttpGet("User/{searchIdentifier}")]
        public async Task<IActionResult> GetUser(string searchIdentifier)
        {
            var user = await _adminService.GetUserByAdmin(searchIdentifier);
            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            return Ok(user);
        }

        [HttpPut("users/{id}/role")]
        public async Task<IActionResult> UpdateUserRole(string id, [FromBody] AdminUserRoleUpdateDto roleUpdateDto)
        {
            if (!ModelState.IsValid) //validerar att NewRole finns i dton
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Admin attempting to update role for user {UserId} to {NewRole}", id,
                roleUpdateDto.NewRole);
            var result =
                await _userService.ChangeUserRole(id, roleUpdateDto.NewRole!); // NewRole är Required så ! är ok här

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                    _logger.LogWarning(
                        "Failed to update role for user {UserId} to {NewRole}. Error: {ErrorCode} - {ErrorDescription}",
                        id, roleUpdateDto.NewRole, error.Code, error.Description);
                }

                if (result.Errors.Any(e => e.Code == "UserNotFound"))
                {
                    return NotFound(ModelState);
                }

                if (result.Errors.Any(e => e.Code == "RoleNotFound" || e.Code == "InvalidRole"))
                {
                    return BadRequest(ModelState);
                }

                return BadRequest(ModelState);
            }

            _logger.LogInformation("Admin successfully updated role for user {UserId} to {NewRole}", id,
                roleUpdateDto.NewRole);
            return Ok(new { Message = $"User {id} role successfully updated to {roleUpdateDto.NewRole}." });
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            _logger.LogInformation("Admin endpoint GetAllUsers accessed.");
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }


        [HttpPost("Meal")]
        public async Task<IActionResult> AddMeal([FromBody] MealCreateDto mealDto)
        {
            var meal = await _mealService.AddMealAsync(mealDto);
            if (meal == null)
            {
                return BadRequest(new { Message = "Failed to create meal" });
            }

            return CreatedAtAction(nameof(AddMeal), new { id = meal.MealId }, meal);
        }

        [HttpPut("UpdateMeal/{id:int}")]
        // [Authorize(Roles = "Admin")] // Om AdminController redan har detta på klassnivå
        public async Task<IActionResult> UpdateMeal(int id, [FromBody] MealUpdateDto mealUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Admin attempting to update meal with ID: {MealId}", id);
          
            var updatedMeal = await _mealService.UpdateMealAsync(id, mealUpdateDto);

            if (updatedMeal == null)
            {
                _logger.LogWarning("Update failed for meal with ID: {MealId}. It might not exist or an error occurred.", id);
                return NotFound(new { Message = $"Failed to update meal with ID {id}. Meal not found or invalid data provided." });
            }

            return Ok(updatedMeal);
        }


        [HttpGet("Orders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }


        [HttpGet("GetOrder/{orderId:int}")]
        public async Task<IActionResult> GetOrderDetailsByAdmin(int orderId)
        {
            var order = await _orderService.GetOrderByIdForAdminAsync(orderId);
            if (order == null) return NotFound(new { Message = $"Order with ID {orderId} not found." });
            return Ok(order);
        }
        [HttpPut("orders/{orderId:int}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] OrderStatusDto statusUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Admin attempting to update status for order ID: {OrderId} to '{NewStatus}'", orderId, statusUpdateDto.NewStatus);

            var success = await _orderService.UpdateOrderStatusAsync(orderId, statusUpdateDto.NewStatus!); // NewStatus är Required, så ! är ok.

            if (!success)
            {
                _logger.LogWarning("Failed to update status for order ID: {OrderId}. Order might not exist or an error occurred.", orderId);
                return NotFound(new { Message = $"Order with ID {orderId} not found or status update failed." });
            }

            _logger.LogInformation("Status for order {OrderId} successfully updated to '{NewStatus}'.", orderId, statusUpdateDto.NewStatus);
            return Ok(new { Message = $"Order {orderId} status successfully updated to {statusUpdateDto.NewStatus}." });
        }

        [HttpDelete("DeleteOrder/{orderId:int}")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var success = await _orderService.DeleteOrderAsync(orderId);
            if (!success) return NotFound(new { Message = $"Order with ID {orderId} not found or deletion failed." });
            return NoContent();
        }
    }
}
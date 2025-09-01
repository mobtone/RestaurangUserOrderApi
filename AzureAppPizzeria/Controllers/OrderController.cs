using AzureAppPizzeria.Core.Interfaces;
using AzureAppPizzeria.Core.Services;
using AzureAppPizzeria.Data.Dtos.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AzureAppPizzeria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService service, ILogger<OrderController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto orderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogError("User ID not found in claims");
                return Unauthorized(new { Message = "User ID not found" });
            }

            _logger.LogInformation("User {UserId} attempting to create a new order", userId);
            var (createdOrder, errorMessage) = await _service.CreateOrderAsync(userId, orderDto);

            if (createdOrder == null)
            {
                _logger.LogWarning("Order creation failed for user {UserId}: {ErrorMessage}", userId, errorMessage);
                //om det är ett specifikt fel från servicen, returnera det.
                //annars ett generiskt fel
                return BadRequest(new { Message = errorMessage ?? "Failed to create order" });
            }

            _logger.LogInformation("Order {OrderId} created successfully for user {UserId}.", createdOrder.OrderId,
                userId);
            // Returnera 201 Created med en länk till den skapade resursen (valfritt men bra praxis)
            return CreatedAtAction(nameof(GetOrderById), new { orderId = createdOrder.OrderId }, createdOrder);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User not authenticated" });
            }

            _logger.LogInformation("Fetching orders for user ID: {UserId}", userId);
            var orders = await _service.GetUserOrdersAsync(userId);
            return Ok(orders);
        }

        [HttpGet("{orderId:int}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    Message = "User not authenticated"
                });
            }

            _logger.LogInformation("Fetching order {OrderId} for user ID: {UserId}", orderId, userId);
            var order = await _service.GetOrderByIdForUserAsync(orderId, userId);

            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found for user ID {UserId} or user does not own it.",
                    orderId, userId);
                return NotFound(new { Message = $"Order with ID {orderId} not found or access denied." });
            }

            return Ok(order);
        }
       
    }

}

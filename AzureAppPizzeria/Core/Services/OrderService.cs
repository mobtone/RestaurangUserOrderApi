using AzureAppPizzeria.Core.Interfaces;
using AzureAppPizzeria.Data;
using AzureAppPizzeria.Data.Dtos.Order;
using AzureAppPizzeria.Data.Entities;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace AzureAppPizzeria.Core.Services
{
    //Denna klass ska använda sig av interface Bonusservice också 
    public class OrderService : IOrderService
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<OrderService> _logger;

        public OrderService(ApplicationContext context, UserManager<ApplicationUser> userManager,
            ILogger<OrderService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<(OrderResponseDto? order, string? errorMessage)> CreateOrderAsync(string userId,
            OrderCreateDto orderDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (null, "User not found");
            }

            var newOrder = new Order
            {
                ApplicationUserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = "Mottagen",
                DiscountApplied = false,
                FreePizzaClaimed = false
            };
            int initialTotalOrderPrice = 0;
            int totalPaidItemsInOrder = 0; //används för att räkna betalda items för bonuspoäng
            int totalItemsInOrder = 0; //används för rabattvillkoret (3+ maträtter)

            List<Meal> orderedMeals = new List<Meal>();

            foreach (var itemDto in orderDto.Items)
            {
                var meal = await _context.Meals
                    .Include(m => m.Category)
                    .FirstOrDefaultAsync(m => m.MealId == itemDto.MealId);
                if (meal == null)
                {
                    return (null, $"Meal with ID {itemDto.MealId} not found");
                }

                if (itemDto.Quantity <= 0)
                {
                    return (null, $"Quantity for meal '{meal.MealName}' must be positive.");
                }

                var orderDetail = new OrderDetails
                {
                    MealId = meal.MealId,
                    Quantity = itemDto.Quantity,
                    PricePerItem = meal.Price
                };
                newOrder.OrderDetails.Add(orderDetail);
                initialTotalOrderPrice += meal.Price * itemDto.Quantity;
                totalItemsInOrder += itemDto.Quantity;
                totalPaidItemsInOrder += itemDto.Quantity;

                for (int i = 0; i < itemDto.Quantity; i++) orderedMeals.Add(meal);
            }

            newOrder.OriginalPrice = initialTotalOrderPrice;
            newOrder.FinalPrice = initialTotalOrderPrice; 


            //premium user logik
            var userRoles = await _userManager.GetRolesAsync(user);
            bool isPremiumUser = userRoles.Contains("PremiumUser");
            _logger.LogInformation("User {UserId} isPremiumUser: {IsPremium}. Roles: {Roles}", userId, isPremiumUser,
                string.Join(",", userRoles));

            if (isPremiumUser)
            {
                _logger.LogInformation("Applying PremiumUser logic for User {UserId}", userId);
                bool userBonusPointsUpdated = false;
                int priceBeforeDiscounts = newOrder.FinalPrice;

                if (orderDto.ClaimFreePizza && user.BonusPoints >= 100)
                {
                    var pizzaInOrder = orderedMeals
                        .Where(m => m.Category?.CategoryName?.Equals("Pizza", StringComparison.OrdinalIgnoreCase) ??
                                    false)
                        .OrderBy(m => m.Price)
                        .FirstOrDefault();

                    if (pizzaInOrder != null)
                    {
                        newOrder.FinalPrice -= pizzaInOrder.Price; //dra av priset för den billigaste pizza
                        if (newOrder.FinalPrice < 0) newOrder.FinalPrice = 0; //priset kan inte vara negativt

                        user.BonusPoints -= 100; //dra av bonuspoäng
                        newOrder.FreePizzaClaimed = true;
                        totalItemsInOrder--; //minska antalet maträtter i ordern eftersom en pizza är gratis
                        userBonusPointsUpdated = true;

                        _logger.LogInformation("PremiumUser {UserId} claimed a free pizza. Price reduced. New Order FinalPrice: {OrderFinalPrice}", userId, newOrder.FinalPrice);
                        priceBeforeDiscounts = newOrder.FinalPrice; // Uppdatera priset som rabatten ska baseras på
                    }
                    else
                    {
                        _logger.LogWarning(
                            "PremiumUser {UserId} tried to claim free pizza (had {UserBonusPoints} points), but no pizza found in the order.",
                            userId, user.BonusPoints);
                    }
                }

                if (totalItemsInOrder >= 3)
                {
                    int currentPriceForDiscount = newOrder.FinalPrice; // Priset som rabatten ska beräknas på
                    int discountValue = (int)(currentPriceForDiscount * 0.20m);

                    newOrder.FinalPrice -= discountValue;
                    newOrder.DiscountAmount = discountValue;
                    newOrder.DiscountApplied = true;
                    _logger.LogInformation("Applied 20% discount ({DiscountValueOren} ören) for PremiumUser {UserId}. New Order FinalPrice: {OrderFinalPrice}",
                        discountValue, userId, newOrder.FinalPrice);
                }

                if (totalPaidItemsInOrder > 0) 
                {
                    int pointsEarned = totalPaidItemsInOrder * 10;
                    user.BonusPoints += pointsEarned;
                    userBonusPointsUpdated = true;
                    _logger.LogInformation(
                        "Awarded {PointsEarned} bonus points to PremiumUser {UserId} for {TotalPaidItemsInOrder} paid items. Total points: {UserBonusPoints}",
                        pointsEarned, userId, totalPaidItemsInOrder, user.BonusPoints);
                }

                if (userBonusPointsUpdated)
                {
                    var updateUserResult = await _userManager.UpdateAsync(user);
                    if (!updateUserResult.Succeeded)
                    {
                        _logger.LogError("Failed to update user {UserId} bonus points. Errors: {Errors}", userId,
                            string.Join(", ", updateUserResult.Errors.Select(e => e.Description)));
                     
                    }
                }
            }
            else
            {
                _logger.LogInformation("User {UserId} is not PremiumUser. No premium benefits applied. orderDto.ClaimFreePizza was {ClaimFreePizzaValue} but will be ignored.",
                    userId, orderDto.ClaimFreePizza);
            }

            _context.Orders.Add(newOrder);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Order {OrderId} created for user {UserId}.",
                    newOrder.OrderId, userId);
            return (MapOrderToResponseDto(newOrder, user), null);
        }

        public async Task<OrderResponseDto?> GetOrderByIdForUserAsync(int orderId, string userId)
        {
            var order = await _context.Orders
                .Include(o => o.ApplicationUser)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Meal)
                .ThenInclude(m => m.Category)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.ApplicationUserId == userId);

            return order == null ? null : MapOrderToResponseDto(order, order.ApplicationUser);
        }

        public async Task<List<OrderResponseDto>> GetUserOrdersAsync(string userId)
        {
            var orders = await _context.Orders
                .Where(o => o.ApplicationUserId == userId)
                .Include(o => o.ApplicationUser)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Meal)
                .ThenInclude(m =>m.Category)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(o => MapOrderToResponseDto(o, o.ApplicationUser)).ToList();
        }
        public async Task<List<OrderResponseDto>> GetAllOrdersAsync()
        {
            _logger.LogInformation("Admin fetching all orders.");
            var orders = await _context.Orders
                .Include(o => o.ApplicationUser)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Meal)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return orders.Select(o => MapOrderToResponseDto(o, o.ApplicationUser)).ToList();
        }

        public async Task<OrderResponseDto?> GetOrderByIdForAdminAsync(int orderId)
        {
            _logger.LogInformation("Admin fetching order with ID: {OrderId}", orderId);
            var order = await _context.Orders
                .Include(o => o.ApplicationUser)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Meal)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
            return order == null ? null : MapOrderToResponseDto(order, order.ApplicationUser);
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found for status update.", orderId);
                return false;
            }

            order.Status = newStatus;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Status for order {OrderId} updated to '{NewStatus}' by admin.", orderId, newStatus);
            return true;
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found for deletion.", orderId);
                return false;
            }
            // Ta bort relaterade OrderDetails? EF Core hanterar detta med Cascade Delete om konfigurerat.
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Order {OrderId} deleted by admin.", orderId);
            return true;
        }
        private OrderResponseDto MapOrderToResponseDto(Order order, ApplicationUser? user)
        {
            return new OrderResponseDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                OriginalTotalPrice = (decimal)order.OriginalPrice / 100.0m, 
                DiscountAmount = (decimal)order.OriginalPrice / 100.0m,   
                FinalTotalPrice = (decimal)order.OriginalPrice / 100.0m,   
                Status = order.Status,
                DiscountApplied = order.DiscountApplied,
                FreePizzaClaimed = order.FreePizzaClaimed,
                UserName = user?.UserName ?? user?.UserName,
                OrderDetails = order.OrderDetails?.Select(od => new OrderDetailDto
                {
                    MealId = od.MealId,
                    MealName = od.Meal?.MealName,
                    Quantity = od.Quantity,
                    PricePerItem = (decimal)od.PricePerItem / 100.0m,
                    TotalPriceForItem = ((decimal)od.PricePerItem / 100.0m) * od.Quantity
                }).ToList() ?? new List<OrderDetailDto>()
            };
        }
    }
}

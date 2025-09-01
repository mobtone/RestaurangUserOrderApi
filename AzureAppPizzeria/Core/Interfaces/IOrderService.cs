using AzureAppPizzeria.Data.Dtos.Order;

namespace AzureAppPizzeria.Core.Interfaces
{
    public interface IOrderService
    {
        //returnerar antingen den skapade ordern eller felmeddelande
        Task<(OrderResponseDto? order, string? errorMessage)> CreateOrderAsync(string userId, OrderCreateDto orderDto);
        Task<List<OrderResponseDto>> GetUserOrdersAsync(string userId);
        Task<OrderResponseDto?> GetOrderByIdForUserAsync(int orderId, string userId); // För att en kund ska kunna se en specifik order
        Task<List<OrderResponseDto>> GetAllOrdersAsync(); //admin: Se alla ordrar
        Task<OrderResponseDto?> GetOrderByIdForAdminAsync(int orderId); //admin: se en specifik order
        Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus); //admin
        Task<bool> DeleteOrderAsync(int orderId); //addmin
    }
}

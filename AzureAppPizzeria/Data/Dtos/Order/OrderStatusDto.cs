using System.ComponentModel.DataAnnotations;

namespace AzureAppPizzeria.Data.Dtos.Order
{
    public class OrderStatusDto
    {
        [Required]
        public string? NewStatus { get; set; } 
    }
}

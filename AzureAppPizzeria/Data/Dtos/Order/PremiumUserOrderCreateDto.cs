using System.ComponentModel.DataAnnotations;

namespace AzureAppPizzeria.Data.Dtos.Order
{
    public class PremiumUserOrderCreateDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "Order must contain at least one item.")]
        public List<OrderItemCreateDto> Items { get; set; } = new List<OrderItemCreateDto>();

        public bool ClaimFreePizza { get; set; } = false; //endast för PremiumUser
    }
}

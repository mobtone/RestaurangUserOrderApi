using System.ComponentModel.DataAnnotations;

namespace AzureAppPizzeria.Data.Dtos.Order
{
    public class RegularUserOrderCreateDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "Order must contain at least one item")]
        public List<OrderItemCreateDto> Items { get; set; } = new List<OrderItemCreateDto>();
    }
}

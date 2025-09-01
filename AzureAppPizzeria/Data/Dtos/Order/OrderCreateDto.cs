using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AzureAppPizzeria.Data.Dtos.Order
{
    public class OrderCreateDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "Beställningen måste ha minst 1 vara")]
        public List<OrderItemCreateDto> Items { get; set; } = new List<OrderItemCreateDto>();

        //För att PremiumUser ska kunna välja att lösa in en gratis pizza eller ej
        [DefaultValue(false)]
        public bool ClaimFreePizza { get; set; } = false;
    }
}

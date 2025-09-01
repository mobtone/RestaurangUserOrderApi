using System.ComponentModel.DataAnnotations;

namespace AzureAppPizzeria.Data.Dtos.Order
{
    public class OrderItemCreateDto
    {
        //dto klass för att skapa en orderrad i en order
        [Required]
        public int MealId { get; set; }
        [Required]
        public int Quantity { get; set; } // antal av maträtten i ordern
    }
}

using System.ComponentModel.DataAnnotations;

namespace AzureAppPizzeria.Data.Dtos.Meal
{
    public class MealUpdateDto
    {
        //mealId skickas in från URL-parametern, inte från DTO-body för en PUT-request

        [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters.")]
        public string? Name { get; set; }

        [StringLength(500, ErrorMessage = "Description can be at most 500 characters.")]
        public string? Description { get; set; }

        [Range(0, 1000000, ErrorMessage = "Price must be between 0 and 1,000,000 ören.")]
        public int? PriceInOren { get; set; } 

        public int? CategoryId { get; set; } 

        // Om null: ändra inte ingredienserna.
        // Om tom lista: ta bort alla ingredienser.
        // Om lista med ID:n: sätt dessa som de nya ingredienserna.
        public List<int>? IngredientIds { get; set; }
    }
}

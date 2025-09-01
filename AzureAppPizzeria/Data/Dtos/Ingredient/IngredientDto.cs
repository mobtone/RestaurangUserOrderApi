using System.ComponentModel.DataAnnotations;

namespace AzureAppPizzeria.Data.Dtos.Ingredient
{
    public class IngredientDto
    {
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }
    }
}

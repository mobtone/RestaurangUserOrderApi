using System.ComponentModel.DataAnnotations;

namespace AzureAppPizzeria.Data.Entities
{
    public class Ingredient
    {
        [Key]
        public int IngredientId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
       
        //Detta ska vara en lista av kopplingstabellen MealIngredients,en ingrediens kan finnas i flera måltider.
        public List<MealIngredient> Ingredients { get; set; } = new List<MealIngredient>();
    }
}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AzureAppPizzeria.Data.Entities
{
    public class Meal
    {
        [Key]
        public int MealId { get; set; }
        [Required]
        [StringLength(100)]
        public string MealName { get; set; }
      
        [StringLength(200)]
        public string Description { get; set; }
        [Required]
        public int Price { get; set; }

        public int CategoryId { get; set; }
        public MealCategory Category { get; set; } = null!;
        public List<MealIngredient> MealIngredients { get; set; } = new List<MealIngredient>();
        public List<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();

    }
}

using System.ComponentModel.DataAnnotations;

namespace AzureAppPizzeria.Data.Entities
{
    public class MealCategory
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string CategoryName { get; set; }

        public List<Meal> Meals { get; set; } = new List<Meal>();
    }
}

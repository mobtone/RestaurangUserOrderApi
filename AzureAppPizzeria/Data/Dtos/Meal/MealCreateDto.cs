using System.ComponentModel.DataAnnotations;

namespace AzureAppPizzeria.Data.Dtos.Meal
{
    public class MealCreateDto
    {
        [StringLength(100)]
        [Required]
        [Display(Name = "Maträttens namn")]
        public string? Name { get; set; }
        [Display(Name = "Beskrivning")]
        [StringLength(500)] 
        public string? Description { get; set; }
        [Required] 
        [Display(Name = "Pris")]
        public int? Price { get; set; }
        [Required]
        [Display(Name = "Kategori Id")]
        public int? CategoryId { get; set; }
        [Display(Name = "Ingrediens Ids")]
        public List<int>? IngredientIds { get; set; } // Om null, inga ändringar i ingredienser
    }
}

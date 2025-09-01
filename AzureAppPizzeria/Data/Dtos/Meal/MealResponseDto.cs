namespace AzureAppPizzeria.Data.Dtos.Meal
{
    public class MealResponseDto
    {
        public int MealId { get; set; }
        public string? MealName { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; } 
        public string? CategoryName { get; set; } 
        public List<string> IngredientNames { get; set; } = new List<string>(); 
    }
}

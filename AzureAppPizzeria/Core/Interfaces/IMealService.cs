using AzureAppPizzeria.Data.Dtos.Meal;

namespace AzureAppPizzeria.Core.Interfaces
{
    public interface IMealService
    {
        Task <MealResponseDto?> AddMealAsync(MealCreateDto mealDto);
        Task<MealResponseDto?> UpdateMealAsync(int id, MealUpdateDto mealDto);
        Task<bool> DeleteMealAsync(int id);
        Task<MealResponseDto?> GetMealByIdAsync(int id);
        Task<List<MealResponseDto>> GetAllMealsAsync();
    }
}

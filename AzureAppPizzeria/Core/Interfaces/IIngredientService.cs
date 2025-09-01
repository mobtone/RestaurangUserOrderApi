using AzureAppPizzeria.Data.Dtos.Ingredient;

namespace AzureAppPizzeria.Core.Interfaces
{
    public interface IIngredientService
    {
        Task<IngredientResponseDto?> CreateIngredientAsync(IngredientDto dto);
        Task<IngredientResponseDto?> UpdateIngredientAsync(int ingredientId, IngredientDto dto);
        Task<bool> DeleteIngredientAsync(int ingredientId);
        Task<IngredientResponseDto?> GetIngredientByIdAsync(int ingredientId);
        Task<List<IngredientResponseDto>> GetAllIngredientsAsync();
    }
}

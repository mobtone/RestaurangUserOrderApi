using AzureAppPizzeria.Core.Interfaces;
using AzureAppPizzeria.Data;
using AzureAppPizzeria.Data.Dtos.Ingredient;
using AzureAppPizzeria.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AzureAppPizzeria.Core.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<IngredientService> _logger;

        public IngredientService(ApplicationContext context, ILogger<IngredientService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IngredientResponseDto?> CreateIngredientAsync(IngredientDto dto)
        {
            if (await _context.Ingredients.AnyAsync(i => i.Name == dto.Name))
            {
                _logger.LogWarning("Trying to create ingredient with an existing name: {IngredientName}", dto.Name);
                //kaastar ett specifikt undantag eller returnera null/felobjekt)
                return null; ;
            }
            var ingredient = new Ingredient { Name = dto.Name };
            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Ingredient {IngredientName} created successfully.", ingredient.Name, ingredient.IngredientId);
            return MapToDto(ingredient);

        }

        public async Task<bool> DeleteIngredientAsync(int ingredientId)
        {
            var ingredient = await _context.Ingredients.Include(i => i.Ingredients)
                .FirstOrDefaultAsync(i => i.IngredientId == ingredientId);
            if (ingredient == null)
            {
                _logger.LogWarning("Ingredient with ID {IngredientId} not found", ingredientId);
                return false;
            }
            if (ingredient.Ingredients != null && ingredient.Ingredients.Any())
            {
                _logger.LogWarning("Attempted to delete ingredient {IngredientId} ('{IngredientName}') which is currently used in meals.", ingredient.IngredientId, ingredient.Name);
                return false; //förhindrar radering om ingrediensen används - då måste måltiden raderas först isåfall
            }
            _context.Ingredients.Remove(ingredient);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Ingredient {IngredientId} ('{IngredientName}') deleted", ingredient.IngredientId, ingredient.Name);
            return true;
        }

        public async Task<List<IngredientResponseDto>> GetAllIngredientsAsync()
        {
            return await _context.Ingredients.Select(i => MapToDto(i)).ToListAsync();
        }

        public async Task<IngredientResponseDto?> GetIngredientByIdAsync(int ingredientId)
        {
            var ingredient = await _context.Ingredients.FindAsync(ingredientId);
            return ingredient == null ? null : MapToDto(ingredient);
        }

        public Task<IngredientResponseDto?> UpdateIngredientAsync(int ingredientId, IngredientDto dto)
        {
            var ingredient = _context.Ingredients.Find(ingredientId);
            if (ingredient == null)
            {
                _logger.LogWarning("Ingredient with ID {IngredientId} not found for update", ingredientId);
                return Task.FromResult<IngredientResponseDto?>(null);
            }
            ingredient.Name = dto.Name;
            _context.Ingredients.Update(ingredient);
            _context.SaveChanges();
            _logger.LogInformation("Ingredient {IngredientId} updated successfully", ingredientId);
            return Task.FromResult(MapToDto(ingredient));
        }

        //en statisk hjälpmtod för att mappa en Ingredient till IngredientResponseDto
        //en separat klass eftersom att mappningen behövs på flera ställen
        private static IngredientResponseDto MapToDto(Ingredient ingredient)
        {
            return new IngredientResponseDto { IngredientId = ingredient.IngredientId, Name = ingredient.Name };
        }
    }
}

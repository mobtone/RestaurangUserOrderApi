using AzureAppPizzeria.Core.Interfaces;
using AzureAppPizzeria.Data;
using AzureAppPizzeria.Data.Dtos.Meal;
using AzureAppPizzeria.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AzureAppPizzeria.Core.Services
{
    public class MealService : IMealService
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<MealService> _logger;

        public MealService(ApplicationContext context, ILogger<MealService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<MealResponseDto?> AddMealAsync(MealCreateDto mealDto)
        {
            if (!await _context.MealCategories.AnyAsync(c => c.CategoryId == mealDto.CategoryId))
            {
                _logger.LogWarning("Category with ID {CategoryId} not found for new meal", mealDto.CategoryId);
                return null;
            }

            var meal = new Meal
            {
                MealName = mealDto.Name,
                Description = mealDto.Description,
                Price = (int)mealDto.Price,
                CategoryId = (int)mealDto.CategoryId
            };
            if (mealDto.IngredientIds != null && mealDto.IngredientIds.Any())
            {
                foreach (var ingredientId in mealDto.IngredientIds.Distinct()) // Distinct för att undvika dubbletter
                {
                    var ingredient = await _context.Ingredients.FindAsync(ingredientId);
                    if (ingredient != null)
                    {
                        meal.MealIngredients.Add(new MealIngredient { IngredientId = ingredient.IngredientId });
                    }
                    else
                    {
                        _logger.LogWarning(
                            "Ingredient with ID {IngredientId} not found when creating meal '{MealName}'. Skipping.",
                            ingredientId, mealDto.Name);
                    }
                }
            }

            _context.Meals.Add(meal);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Meal '{MealName}' created with ID {MealId}", meal.MealName, meal.MealId);

            var createdMealWithDetails = await GetMealEntityByIdAsync(meal.MealId);
            return MapMealToResponseDto(createdMealWithDetails!);
        }

        public Task<bool> DeleteMealAsync(int id)
        {
            _logger.LogInformation("Attempting to delete meal with ID: {MealId}", id);

            var meal = _context.Meals.Find(id);
            if (meal == null)
            {
                _logger.LogWarning("Meal with ID {MealId} not found for deletion", id);
                return Task.FromResult(false);
            }

            _context.Meals.Remove(meal);
            return _context.SaveChangesAsync().ContinueWith(t =>
            {
                if (t.IsCompletedSuccessfully)
                {
                    _logger.LogInformation("Meal with ID {MealId} deleted successfully", id);
                    return true;
                }
                else
                {
                    _logger.LogError(t.Exception, "Error deleting meal with ID {MealId}", id);
                    return false;
                }
            });
        }

        public async Task<List<MealResponseDto>> GetAllMealsAsync()
        {
            _logger.LogInformation("Fetching all meals");
            var meals = await _context.Meals
                .Include(m => m.Category)
                .Include(m => m.MealIngredients)
                .ThenInclude(mi => mi.Ingredient)
                .OrderBy(m => m.Category.CategoryName)
                .ThenBy(m => m.MealName)
                .ToListAsync();

            return meals.Select(MapMealToResponseDto).ToList();
        }


        public async Task<MealResponseDto?> GetMealByIdAsync(int mealId)
        {
            _logger.LogInformation("Fetching meal with ID: {MealId}", mealId);
            var mealEntity = await GetMealEntityByIdAsync(mealId);
            if (mealEntity == null)
            {
                _logger.LogWarning("Meal with ID: {MealId} not found (when fetching for DTO).", mealId);
                return null;
            }
            return MapMealToResponseDto(mealEntity);
        }

        public async Task<MealResponseDto?> UpdateMealAsync(int mealId, MealUpdateDto dto)
        {
            _logger.LogInformation("Attempting to update meal with ID: {MealId}. Incoming DTO: Name='{DtoName}', Price='{DtoPrice}', CategoryId='{DtoCategory}', IngredientIdsCount='{DtoIngredientsCount}'",
                mealId, dto.Name, dto.PriceInOren, dto.CategoryId, dto.IngredientIds?.Count);

            var mealToUpdate = await GetMealEntityByIdAsync(mealId);

            //if (mealToUpdate == null)
            //{
            //    _logger.LogWarning("MealService.UpdateMealAsync: Meal with ID {MealId} was NOT found in database. Aborting update.", mealId);
            //    return null; // Orsak 1 till att controller får 404
            //}
            _logger.LogInformation("MealService.UpdateMealAsync: Found meal '{ExistingMealName}' (ID: {MealId}) to update.", mealToUpdate.MealName, mealId);

            bool hasChanges = false;

            // Uppdatera MealName
            if (dto.Name != null && !string.IsNullOrWhiteSpace(dto.Name) && mealToUpdate.MealName != dto.Name)
            {
                _logger.LogInformation("Updating MealName for MealId {MealId} from '{OldName}' to '{NewName}'", mealId, mealToUpdate.MealName, dto.Name);
                mealToUpdate.MealName = dto.Name;
                hasChanges = true;
            }

            // Uppdatera Description (tillåt att sätta till tom sträng om det är avsikten)
            if (dto.Description != null && mealToUpdate.Description != dto.Description)
            {
                _logger.LogInformation("Updating Description for MealId {MealId}", mealId);
                mealToUpdate.Description = dto.Description;
                hasChanges = true;
            }

            // Uppdatera Price
            if (dto.PriceInOren.HasValue && mealToUpdate.Price != dto.PriceInOren.Value)
            {
                _logger.LogInformation("Updating Price for MealId {MealId} from {OldPrice} to {NewPrice}", mealId, mealToUpdate.Price, dto.PriceInOren.Value);
                mealToUpdate.Price = dto.PriceInOren.Value;
                hasChanges = true;
            }

            // Uppdatera CategoryId
            if (dto.CategoryId.HasValue && mealToUpdate.CategoryId != dto.CategoryId.Value)
            {
                _logger.LogInformation("Attempting to update CategoryId for MealId {MealId} from {OldCategoryId} to {NewCategoryId}", mealId, mealToUpdate.CategoryId, dto.CategoryId.Value);
                var categoryExists = await _context.MealCategories.AnyAsync(c => c.CategoryId == dto.CategoryId.Value);
                if (!categoryExists)
                {
                    _logger.LogWarning("UpdateMealAsync: New Category ID {ProvidedCategoryId} from DTO does NOT exist. Aborting update for MealId {MealId}.", dto.CategoryId.Value, mealId);
                    // Returnera ett felobjekt eller kasta undantag istället för null för att ge mer info till controllern
                    // För nu, returnerar vi null som tidigare, vilket leder till 404 i controllern.
                    return null; // Orsak 2 till att controller får 404
                }
                mealToUpdate.CategoryId = dto.CategoryId.Value;
                hasChanges = true;
                _logger.LogInformation("CategoryId updated to {NewCategoryId} for MealId {MealId}", mealToUpdate.CategoryId, mealId);
            }

            // Hantera uppdatering av ingredienser
            if (dto.IngredientIds != null) // Om IngredientIds är null, rör vi inte ingredienserna.
            {
                _logger.LogInformation("Processing ingredient updates for MealId {MealId}. Received {Count} ingredient IDs.", mealId, dto.IngredientIds.Count);
                hasChanges = true; // Även en tom lista för att ta bort alla är en ändring.

                // Rensa befintliga kopplingar. EF Core spårar detta som borttagningar.
                // Viktigt att mealToUpdate.MealIngredients är laddad med .Include() i GetMealEntityByIdAsync.
                mealToUpdate.MealIngredients.Clear();
                _logger.LogInformation("Cleared existing MealIngredients for MealId {MealId}.", mealId);

                if (dto.IngredientIds.Any())
                {
                    _logger.LogInformation("Adding new ingredients for MealId {MealId}: [{NewIngredientIds}]", mealId, string.Join(",", dto.IngredientIds));
                    foreach (var ingredientId in dto.IngredientIds.Distinct())
                    {
                        var ingredient = await _context.Ingredients.FindAsync(ingredientId);
                        if (ingredient != null)
                        {
                            mealToUpdate.MealIngredients.Add(new MealIngredient { IngredientId = ingredient.IngredientId /* MealId sätts av EF Core */ });
                            _logger.LogInformation("Added IngredientId {IngredientId} to MealId {MealId}.", ingredientId, mealId);
                        }
                        else
                        {
                            _logger.LogWarning("Ingredient with ID {IngredientId} not found while updating meal {MealId}. Skipping this ingredient.", ingredientId, mealId);
                            // VIKTIGT: Om en ingrediens inte hittas, bör du överväga om hela uppdateringen ska misslyckas.
                            // Att bara skippa den kan leda till en maträtt med färre ingredienser än admin avsåg.
                            // return null; // Exempel: Avbryt om en ingrediens är ogiltig.
                        }
                    }
                }
                else
                {
                    _logger.LogInformation("Received empty IngredientIds list for MealId {MealId}. All ingredients have been removed.", mealId);
                }
            }

            if (!hasChanges)
            {
                _logger.LogInformation("No actual changes detected for MealId {MealId} based on DTO input. Returning current data.", mealId);
                return MapMealToResponseDto(mealToUpdate); // Returnera den oförändrade (men hittade) maträtten
            }

            _logger.LogInformation("Attempting to SaveChanges for MealId {MealId}.", mealId);
            try
            {
                int affectedRows = await _context.SaveChangesAsync();
                _logger.LogInformation("SaveChangesAsync completed for MealId {MealId}. Affected rows: {AffectedRows}", mealId, affectedRows);
                if (affectedRows == 0 && hasChanges)
                {
                    // Detta kan hända om inga faktiska ändringar spårades trots att hasChanges är true,
                    // eller om det finns ett concurrency-problem som inte kastade DbUpdateConcurrencyException.
                    _logger.LogWarning("SaveChangesAsync reported 0 affected rows for MealId {MealId}, but hasChanges was true. Concurrency issue or no actual data modification detected by EF Core.", mealId);
                    // Du kan välja att se detta som ett fel eller ej.
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "A concurrency error occurred while updating meal {MealId}. Message: {ErrorMessage}", mealId, ex.Message);
                return null; // Orsak 3 till att controller får 404
            }
            catch (DbUpdateException ex) // Fånga mer specifika databasfel
            {
                _logger.LogError(ex, "A database update error occurred while saving changes for meal {MealId}. InnerException: {InnerExceptionMessage}. Message: {ErrorMessage}", mealId, ex.InnerException?.Message, ex.Message);
                return null; // Orsak 4 till att controller får 404
            }
            catch (Exception ex) // Generell felhanterare
            {
                _logger.LogError(ex, "An unexpected error occurred while saving changes for meal {MealId}. Message: {ErrorMessage}", mealId, ex.Message);
                return null; // Orsak 5 till att controller får 404
            }

            _logger.LogInformation("Fetching updated meal details for MealId {MealId} to return in response.", mealId);
            var updatedMealWithDetails = await GetMealEntityByIdAsync(mealId); // Hämta igen för att få färsk data
            if (updatedMealWithDetails == null)
            {
                _logger.LogError("CRITICAL: Failed to retrieve meal with ID {MealId} after supposedly successful SaveChanges. This indicates a serious issue.", mealId);
                return null; // Orsak 6 till att controller får 404
            }
            return MapMealToResponseDto(updatedMealWithDetails);
        }
        private async Task<Meal?> GetMealEntityByIdAsync(int mealId)
        {
            return await _context.Meals
                .Include(m => m.Category)
                .Include(m => m.MealIngredients)
                .ThenInclude(mi => mi.Ingredient)
                .FirstOrDefaultAsync(m => m.MealId == mealId);
        }

        // Privat hjälpmetod för mappning från Meal-entitet till MealResponseDto
        private MealResponseDto MapMealToResponseDto(Meal meal)
        {
            if (meal == null) throw new ArgumentNullException(nameof(meal), "Cannot map a null meal entity.");

            return new MealResponseDto
            {
                MealId = meal.MealId,
                MealName = meal.MealName,
                Description = meal.Description,
                Price = (decimal)meal.Price / 100.0m, // Konvertera från ören
                CategoryName = meal.Category?.CategoryName,
                IngredientNames = meal.MealIngredients?
                    .Select(mi => mi.Ingredient?.Name)
                    .Where(name => name != null)
                    .Select(name => name!) 
                    .ToList() ?? new List<string>()
            };
        }

    }
}


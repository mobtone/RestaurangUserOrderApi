using AzureAppPizzeria.Core.Interfaces;
using AzureAppPizzeria.Core.Services;
using AzureAppPizzeria.Data.Dtos.Ingredient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureAppPizzeria.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;
        private readonly ILogger<IngredientController> _logger;
        public IngredientController(IIngredientService ingredientService, ILogger<IngredientController> logger)
        {
            _ingredientService = ingredientService;
            _logger = logger;
        }

        [HttpGet("Ingredients")]
        public async Task<IActionResult> GetAllIngredients()
        {
            try
            {
                _logger.LogInformation("Endpoint GetAllIngredients accessed");
                var ingredients = await _ingredientService.GetAllIngredientsAsync();
                return Ok(ingredients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching ingredients");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("Ingredients/{id:int}")]
        public async Task<IActionResult> GetIngredient(int id)
        {
            _logger.LogInformation("Endpoint GetIngredient accessed with ID: {IngredientId}", id);
            var ingredient = await _ingredientService.GetIngredientByIdAsync(id);
            if (ingredient == null)
            {
                _logger.LogWarning("Ingredient with ID {IngredientId} not found when accessed via endpoint.", id);
                return NotFound(new { Message = $"Ingredient with ID {id} not found." });
            }
            return Ok(ingredient);
        }
        [HttpPost("AddIngredient")]
        public async Task<IActionResult> CreateIngredient([FromBody] IngredientDto ingredientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var createdIngredient = await _ingredientService.CreateIngredientAsync(ingredientDto);
            if (createdIngredient == null)
            {
                return BadRequest(new { Message = "Failed to create ingredient" });
            }
            return CreatedAtAction(nameof(GetIngredient), new { id = createdIngredient.IngredientId }, createdIngredient);
        }
        [HttpPut("ingredients/{id:int}")]
        public async Task<IActionResult> UpdateIngredient(int id, [FromBody] IngredientDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updatedIngredient = await _ingredientService.UpdateIngredientAsync(id, dto);
            if (updatedIngredient == null) return NotFound(new { Message = $"Ingredient with ID {id} not found or new name conflicts." });
            return Ok(updatedIngredient);
        }

        [HttpDelete("DeleteIngredient/{id:int}")]
        public async Task<IActionResult> DeleteIngredient(int id)
        {
            var success = await _ingredientService.DeleteIngredientAsync(id);
            if (!success) return NotFound(new { Message = $"Ingredient with ID {id} not found or cannot be deleted (in use)" });
            return NoContent();
        }
    }
}

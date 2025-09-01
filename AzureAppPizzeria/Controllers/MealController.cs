using AzureAppPizzeria.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureAppPizzeria.Controllers
{
    //Skyddad för admin - bara admin kan använda denna endpoint 
    //för att hantera maträtter/meals lägga till/uppdatera pizzor/maträtter

    [Route("api/[controller]")]
    [ApiController]

   
    public class MealController : ControllerBase
    {
        private readonly IMealService _mealService;
        private readonly ILogger<MealController> _logger;
        public MealController(IMealService mealService, ILogger<MealController> logger)
        {
            _mealService = mealService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMeals()
        {
            try
            {
                _logger.LogInformation("endpoint GetALlMeals accessed");
                var meals = await _mealService.GetAllMealsAsync();
                return Ok(meals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching meals");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetMeal(int id)
        {
            _logger.LogInformation("Endpoint GetMeal accessed with ID: {MealId}", id);
            var meal = await _mealService.GetMealByIdAsync(id);
            if (meal == null)
            {
                _logger.LogWarning("Meal with ID {MealId} not found when accessed via endpoint.", id);
                return NotFound(new { Message = $"Meal with ID {id} not found." });
            }
            return Ok(meal);
        }
    }
}

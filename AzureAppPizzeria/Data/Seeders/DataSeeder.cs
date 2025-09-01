//using AzureAppPizzeria.Data.Entities;
//using Microsoft.EntityFrameworkCore;

//namespace AzureAppPizzeria.Data.Seeders
//{
//    public static class DataSeeder
//    {
//        using AzureAppPizzeria.Data; // För ApplicationContext
//using AzureAppPizzeria.Data.Entities;
//using Microsoft.EntityFrameworkCore; // För EnsureCreated, ToListAsync etc.
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace AzureAppPizzeria.Extensions 
//    {
//        public static class DataSeeder
//        {
//            public static async Task SeedMenuDataAsync(IServiceProvider serviceProvider)
//            {
//                // Använd en scope för att hämta DbContext korrekt
//                using (var scope = serviceProvider.CreateScope())
//                {
//                    var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
//                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<DataSeeder>>();

//                    try
//                    {
//                        // Valfritt: Om du vill säkerställa att databasen är skapad (kan vara bra för första körningen)
//                        // context.Database.EnsureCreated(); // Denna skapar databasen baserat på modellen om den inte finns,
//                        // men kör INTE migreringar. Använd med försiktighet.
//                        // Om du alltid kör migreringar först, är denna inte nödvändig.

//                        logger.LogInformation("Starting to seed menu data...");

//                        // Steg 1: Seeda Kategorier (om de inte redan finns)
//                        if (!await context.MealCategories.AnyAsync())
//                        {
//                            var categories = new List<MealCategory>
//                        {
//                            new MealCategory { CategoryName = "Pizza" },
//                            new MealCategory { CategoryName = "Pasta" },
//                            new MealCategory { CategoryName = "Sallad" }
//                            // Lägg till fler kategorier om det behövs
//                        };
//                            await context.MealCategories.AddRangeAsync(categories);
//                            await context.SaveChangesAsync();
//                            logger.LogInformation("Seeded MealCategories.");
//                        }
//                        else
//                        {
//                            logger.LogInformation("MealCategories already exist, skipping seeding.");
//                        }

//                        // Steg 2: Seeda Ingredienser (om de inte redan finns)
//                        if (!await context.Ingredients.AnyAsync())
//                        {
//                            var ingredients = new List<Ingredient>
//                        {
//                            new Ingredient { Name = "Tomatsås" },
//                            new Ingredient { Name = "Ost" },
//                            new Ingredient { Name = "Skinka" },
//                            new Ingredient { Name = "Champinjoner" },
//                            new Ingredient { Name = "Ananas" },
//                            new Ingredient { Name = "Kebabkött" },
//                            new Ingredient { Name = "Isbergssallad" },
//                            new Ingredient { Name = "Tomat" },
//                            new Ingredient { Name = "Gurka" },
//                            new Ingredient { Name = "Lök" },
//                            new Ingredient { Name = "Feferoni" },
//                            new Ingredient { Name = "Kebabsås mild" },
//                            new Ingredient { Name = "Kebabsås stark" },
//                            new Ingredient { Name = "Räkor" },
//                            new Ingredient { Name = "Musslor" },
//                            new Ingredient { Name = "Kronärtskocka" },
//                            new Ingredient { Name = "Oliver" },
//                            new Ingredient { Name = "Paprika" },
//                            new Ingredient { Name = "Spaghetti" },
//                            new Ingredient { Name = "Köttfärssås" },
//                            new Ingredient { Name = "Kyckling" },
//                            new Ingredient { Name = "Curry" },
//                            new Ingredient { Name = "Gräslök" },
//                            new Ingredient { Name = "Krutonger" },
//                            new Ingredient { Name = "Caesardressing" }
//                            // Lägg till alla ingredienser du kan komma på
//                        };
//                            await context.Ingredients.AddRangeAsync(ingredients);
//                            await context.SaveChangesAsync();
//                            logger.LogInformation("Seeded Ingredients.");
//                        }
//                        else
//                        {
//                            logger.LogInformation("Ingredients already exist, skipping seeding.");
//                        }

//                        // Steg 3: Seeda Maträtter (om de inte redan finns)
//                        // Detta är lite mer komplext eftersom vi behöver koppla till kategorier och ingredienser
//                        if (!await context.Meals.AnyAsync())
//                        {
//                            // Hämta de skapade kategorierna och ingredienserna för att kunna referera till deras ID:n
//                            var pizzaCategory = await context.MealCategories.FirstOrDefaultAsync(c => c.CategoryName == "Pizza");
//                            var pastaCategory = await context.MealCategories.FirstOrDefaultAsync(c => c.CategoryName == "Pasta");
//                            var salladCategory = await context.MealCategories.FirstOrDefaultAsync(c => c.CategoryName == "Sallad");

//                            // Hämta några ingredienser
//                            var tomatsas = await context.Ingredients.FirstOrDefaultAsync(i => i.Name == "Tomatsås");
//                            var ost = await context.Ingredients.FirstOrDefaultAsync(i => i.Name == "Ost");
//                            var skinka = await context.Ingredients.FirstOrDefaultAsync(i => i.Name == "Skinka");
//                            var ananas = await context.Ingredients.FirstOrDefaultAsync(i => i.Name == "Ananas");
//                            var champinjoner = await context.Ingredients.FirstOrDefaultAsync(i => i.Name == "Champinjoner");
//                            var kebabkott = await context.Ingredients.FirstOrDefaultAsync(i => i.Name == "Kebabkött");
//                            var isberg = await context.Ingredients.FirstOrDefaultAsync(i => i.Name == "Isbergssallad");
//                            var tomat = await context.Ingredients.FirstOrDefaultAsync(i => i.Name == "Tomat");
//                            var gurka = await context.Ingredients.FirstOrDefaultAsync(i => i.Name == "Gurka");
//                            var lok = await context.Ingredients.FirstOrDefaultAsync(i => i.Name == "Lök");
//                            var kebabsasMild = await context.Ingredients.FirstOrDefaultAsync(i => i.Name == "Kebabsås mild");
//                            var spaghetti = await context.Ingredients.FirstOrDefaultAsync(i => i.Name == "Spaghetti");
//                            var kottfarssas = await context.Ingredients.FirstOrDefaultAsync(i => i.Name == "Köttfärssås");
//                            var kyckling = await context.Ingredients.FirstOrDefaultAsync(i => i.Name == "Kyckling");
//                            var krutonger = await context.Ingredients.FirstOrDefaultAsync(i => i.Name == "Krutonger");
//                            var caesardressing = await context.Ingredients.FirstOrDefaultAsync(i => i.Name == "Caesardressing");


//                            if (pizzaCategory == null || pastaCategory == null || salladCategory == null ||
//                                tomatsas == null || ost == null /* ... kontrollera alla nödvändiga ingredienser ... */)
//                            {
//                                logger.LogError("Could not seed meals because some prerequisite categories or ingredients were not found.");
//                                return;
//                            }

//                            var meals = new List<Meal>
//                        {
//                            new Meal
//                            {
//                                MealName = "Margherita",
//                                Description = "Klassisk pizza med tomatsås och ost.",
//                                Price = 85,
//                                CategoryId = pizzaCategory.CategoryId,
//                                MealIngredients = new List<MealIngredient>
//                                {
//                                    new MealIngredient { IngredientId = tomatsas.IngredientId },
//                                    new MealIngredient { IngredientId = ost.IngredientId }
//                                }
//                            },
//                            new Meal
//                            {
//                                MealName = "Vesuvio",
//                                Description = "Pizza med tomatsås, ost och skinka.",
//                                Price = 90, 
//                                CategoryId = pizzaCategory.CategoryId,
//                                MealIngredients = new List<MealIngredient>
//                                {
//                                    new MealIngredient { IngredientId = tomatsas.IngredientId },
//                                    new MealIngredient { IngredientId = ost.IngredientId },
//                                    new MealIngredient { IngredientId = skinka.IngredientId }
//                                }
//                            },
//                            new Meal
//                            {
//                                MealName = "Hawaii",
//                                Description = "Pizza med tomatsås, ost, skinka och ananas.",
//                                Price = 95,
//                                CategoryId = pizzaCategory.CategoryId,
//                                MealIngredients = new List<MealIngredient>
//                                {
//                                    new MealIngredient { IngredientId = tomatsas.IngredientId },
//                                    new MealIngredient { IngredientId = ost.IngredientId },
//                                    new MealIngredient { IngredientId = skinka.IngredientId },
//                                    new MealIngredient { IngredientId = ananas.IngredientId }
//                                }
//                            },
//                            new Meal
//                            {
//                                MealName = "Kebabpizza",
//                                Description = "Pizza med tomatsås, ost, kebabkött, lök, isbergssallad och kebabsås.",
//                                Price = 110,
//                                CategoryId = pizzaCategory.CategoryId,
//                                MealIngredients = new List<MealIngredient>
//                                {
//                                    new MealIngredient { IngredientId = tomatsas.IngredientId },
//                                    new MealIngredient { IngredientId = ost.IngredientId },
//                                    new MealIngredient { IngredientId = kebabkott.IngredientId },
//                                    new MealIngredient { IngredientId = lok.IngredientId },
//                                    new MealIngredient { IngredientId = isberg.IngredientId },
//                                    new MealIngredient { IngredientId = kebabsasMild.IngredientId }
//                                }
//                            },
//                            new Meal
//                            {
//                                MealName = "Spaghetti Bolognese",
//                                Description = "Klassisk spaghetti med köttfärssås.",
//                                Price = 105,
//                                CategoryId = pastaCategory.CategoryId,
//                                MealIngredients = new List<MealIngredient> // Ingredienser i själva rätten
//                                {
//                                    new MealIngredient { IngredientId = spaghetti.IngredientId },
//                                    new MealIngredient { IngredientId = kottfarssas.IngredientId }
//                                }
//                            },
//                            new Meal
//                            {
//                                MealName = "Caesarsallad",
//                                Description = "Sallad med kyckling, romansallad, krutonger, parmesan och caesardressing.",
//                                Price = 115,
//                                CategoryId = salladCategory.CategoryId,
//                                MealIngredients = new List<MealIngredient>
//                                {
//                                    new MealIngredient { IngredientId = isberg.IngredientId }, // Använd romansallad om du har den
//                                    new MealIngredient { IngredientId = kyckling.IngredientId },
//                                    new MealIngredient { IngredientId = krutonger.IngredientId },
//                                    new MealIngredient { IngredientId = caesardressing.IngredientId }
//                                    // Parmesan kan vara en egen ingrediens
//                                }
//                            }
//                        };

//                            await context.Meals.AddRangeAsync(meals);
//                            await context.SaveChangesAsync(); // Detta sparar Meals OCH de relaterade MealIngredients
//                            logger.LogInformation("Seeded Meals with their ingredients.");
//                        }
//                        else
//                        {
//                            logger.LogInformation("Meals already exist, skipping seeding.");
//                        }

//                        logger.LogInformation("Menu data seeding completed.");
//                    }
//                    catch (Exception ex)
//                    {
//                        logger.LogError(ex, "An error occurred while seeding menu data.");
//                        // Överväg att kasta om felet om seedingen är kritisk
//                    }
//                }
//            }
//        }
//    }
//}
//}

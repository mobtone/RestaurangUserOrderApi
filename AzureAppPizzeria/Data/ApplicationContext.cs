using System.Net.Mime;
using AzureAppPizzeria.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace AzureAppPizzeria.Data
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public virtual DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public virtual DbSet<Ingredient> Ingredients { get; set; }
        public virtual DbSet<MealCategory> MealCategories { get; set; }
        public virtual DbSet<Meal> Meals { get; set; }
        public virtual DbSet<MealIngredient> MealIngredients { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetails> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<MealIngredient>()
                .HasOne(mi => mi.Meal)
                .WithMany(m => m.MealIngredients)
                .HasForeignKey(mi => mi.MealId);

            builder.Entity<MealIngredient>()
                .HasOne(mi => mi.Ingredient)
                .WithMany(i => i.Ingredients)
                .HasForeignKey(mi => mi.IngredientId);

            builder.Entity<MealIngredient>()
            .HasKey(mi => new { mi.MealId, mi.IngredientId });

            //ApplicationUser till Order (one-to-many)
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.ApplicationUser)
                .HasForeignKey(o => o.ApplicationUserId);

            //MealCategory till Meal (one-to-many)
            builder.Entity<MealCategory>()
                .HasMany(c => c.Meals)
                .WithOne(m => m.Category)
                .HasForeignKey(m => m.CategoryId);

            //Order till OrderDetail (one-to-many)
            builder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId);

            //Meal till OrderDetail (one-to-many)
            builder.Entity<Meal>()
                .HasMany(m => m.OrderDetails)
                .WithOne(od => od.Meal)
                .HasForeignKey(od => od.MealId);

            //När jag använder HasData måste jag ange primärnyckelarna (som CategoryId = 1, IngredientId = 1, MealId = 1) manuellt.
            //EF Core kommer inte att autogenerera dem för HasData eftersom denna data infogas innan identitetsfunktionalitet som autoincrement
            //är aktiva för dessa rader under migrationen.därför måste jag ange dem manuellt innan migrationen.
            //när jag refererar till en främmande nyckel som ex CategoryId i Meal eller MealId och IngredientId i MealIngredient
            //använder jag de idn jag har definierat för de relaterade entiteterna

            // 1. Seed MealCategories
            var pizzaCategoryId = 1;
            var pastaCategoryId = 2;
            var salladCategoryId = 3;

            builder.Entity<MealCategory>().HasData(
                new MealCategory { CategoryId = pizzaCategoryId, CategoryName = "Pizza" },
                new MealCategory { CategoryId = pastaCategoryId, CategoryName = "Pasta" },
                new MealCategory { CategoryId = salladCategoryId, CategoryName = "Sallad" }
            );

            // 2. Seed Ingredients
            //IDn tilldelas manuellt till HasData, eftersom de inte genereras av databasen
            //förrän efter SaveChanges, vilket HasData kringgår för just detta syfte
            var tomatsasId = 1; var ostId = 2; var skinkaId = 3; var ananasId = 4;
            var champinjonerId = 5; var kebabkottId = 6; var isbergId = 7; var tomatId = 8;
            var gurkaId = 9; var lokId = 10; var feferoniId = 11; var kebabsasMildId = 12;
            var kebabsasStarkId = 13; var rakorId = 14; var musslorId = 15; var kronartskockaId = 16;
            var oliverId = 17; var paprikaId = 18; var spaghettiId = 19; var kottfarssasId = 20;
            var kycklingId = 21; var curryId = 22; var graslokId = 23; var krutongerId = 24;
            var caesardressingId = 25;

            builder.Entity<Ingredient>().HasData(
                new Ingredient { IngredientId = tomatsasId, Name = "Tomatsås" },
                new Ingredient { IngredientId = ostId, Name = "Ost" },
                new Ingredient { IngredientId = skinkaId, Name = "Skinka" },
                new Ingredient { IngredientId = ananasId, Name = "Ananas" },
                new Ingredient { IngredientId = champinjonerId, Name = "Champinjoner" },
                new Ingredient { IngredientId = kebabkottId, Name = "Kebabkött" },
                new Ingredient { IngredientId = isbergId, Name = "Isbergssallad" },
                new Ingredient { IngredientId = tomatId, Name = "Tomat" },
                new Ingredient { IngredientId = gurkaId, Name = "Gurka" },
                new Ingredient { IngredientId = lokId, Name = "Lök" },
                new Ingredient { IngredientId = feferoniId, Name = "Feferoni" },
                new Ingredient { IngredientId = kebabsasMildId, Name = "Kebabsås mild" },
                new Ingredient { IngredientId = kebabsasStarkId, Name = "Kebabsås stark" },
                new Ingredient { IngredientId = rakorId, Name = "Räkor" },
                new Ingredient { IngredientId = musslorId, Name = "Musslor" },
                new Ingredient { IngredientId = kronartskockaId, Name = "Kronärtskocka" },
                new Ingredient { IngredientId = oliverId, Name = "Oliver" },
                new Ingredient { IngredientId = paprikaId, Name = "Paprika" },
                new Ingredient { IngredientId = spaghettiId, Name = "Spaghetti" },
                new Ingredient { IngredientId = kottfarssasId, Name = "Köttfärssås" },
                new Ingredient { IngredientId = kycklingId, Name = "Kyckling" },
                new Ingredient { IngredientId = curryId, Name = "Curry" },
                new Ingredient { IngredientId = graslokId, Name = "Gräslök" },
                new Ingredient { IngredientId = krutongerId, Name = "Krutonger" },
                new Ingredient { IngredientId = caesardressingId, Name = "Caesardressing" }
            );

            // 3. Seed Meals
            //tilldela MealId manuellt
            var margheritaId = 1; var vesuvioId = 2; var hawaiiId = 3;
            var kebabpizzaId = 4; var bologneseId = 5; var caesarSalladId = 6;

            builder.Entity<Meal>().HasData(
                new Meal { MealId = margheritaId, MealName = "Margherita", Description = "Klassisk pizza med tomatsås och ost.", Price = 8500, CategoryId = pizzaCategoryId },
                new Meal { MealId = vesuvioId, MealName = "Vesuvio", Description = "Pizza med tomatsås, ost och skinka.", Price = 9000, CategoryId = pizzaCategoryId },
                new Meal { MealId = hawaiiId, MealName = "Hawaii", Description = "Pizza med tomatsås, ost, skinka och ananas.", Price = 9500, CategoryId = pizzaCategoryId },
                new Meal { MealId = kebabpizzaId, MealName = "Kebabpizza", Description = "Pizza med tomatsås, ost, kebabkött, lök, isbergssallad och kebabsås.", Price = 11000, CategoryId = pizzaCategoryId },
                new Meal { MealId = bologneseId, MealName = "Spaghetti Bolognese", Description = "Klassisk spaghetti med köttfärssås.", Price = 10500, CategoryId = pastaCategoryId },
                new Meal { MealId = caesarSalladId, MealName = "Caesarsallad", Description = "Sallad med kyckling, romansallad, krutonger, parmesan och caesardressing.", Price = 11500, CategoryId = salladCategoryId }
            );

            // 4. Seed MealIngredients (kopplingstabellen)
            // Här specificerar du vilka ingredienser som tillhör vilka maträtter
            builder.Entity<MealIngredient>().HasData(
                // Margherita
                new MealIngredient { MealId = margheritaId, IngredientId = tomatsasId },
                new MealIngredient { MealId = margheritaId, IngredientId = ostId },
                // Vesuvio
                new MealIngredient { MealId = vesuvioId, IngredientId = tomatsasId },
                new MealIngredient { MealId = vesuvioId, IngredientId = ostId },
                new MealIngredient { MealId = vesuvioId, IngredientId = skinkaId },
                //Hawaiipizaz
                new MealIngredient { MealId = hawaiiId, IngredientId = tomatsasId },
                new MealIngredient { MealId = hawaiiId, IngredientId = ostId },
                new MealIngredient { MealId = hawaiiId, IngredientId = skinkaId },
                new MealIngredient { MealId = hawaiiId, IngredientId = ananasId },
                //Kebabpizza
                new MealIngredient { MealId = kebabpizzaId, IngredientId = tomatsasId },
                new MealIngredient { MealId = kebabpizzaId, IngredientId = ostId },
                new MealIngredient { MealId = kebabpizzaId, IngredientId = kebabkottId },
                new MealIngredient { MealId = kebabpizzaId, IngredientId = lokId },
                new MealIngredient { MealId = kebabpizzaId, IngredientId = isbergId },
                new MealIngredient { MealId = kebabpizzaId, IngredientId = kebabsasMildId },
                //spaghetti bolognese
                new MealIngredient { MealId = bologneseId, IngredientId = spaghettiId },
                new MealIngredient { MealId = bologneseId, IngredientId = kottfarssasId },
                //Caesarsallad
                new MealIngredient { MealId = caesarSalladId, IngredientId = isbergId },
                new MealIngredient { MealId = caesarSalladId, IngredientId = kycklingId },
                new MealIngredient { MealId = caesarSalladId, IngredientId = krutongerId },
                new MealIngredient { MealId = caesarSalladId, IngredientId = caesardressingId }
            );
        }
    }
}

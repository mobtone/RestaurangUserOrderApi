using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AzureAppPizzeria.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialMenuData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MealIngredients",
                table: "MealIngredients");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MealIngredients",
                table: "MealIngredients",
                columns: new[] { "MealId", "IngredientId" });

            migrationBuilder.InsertData(
                table: "Ingredients",
                columns: new[] { "IngredientId", "Name" },
                values: new object[,]
                {
                    { 1, "Tomatsås" },
                    { 2, "Ost" },
                    { 3, "Skinka" },
                    { 4, "Ananas" },
                    { 5, "Champinjoner" },
                    { 6, "Kebabkött" },
                    { 7, "Isbergssallad" },
                    { 8, "Tomat" },
                    { 9, "Gurka" },
                    { 10, "Lök" },
                    { 11, "Feferoni" },
                    { 12, "Kebabsås mild" },
                    { 13, "Kebabsås stark" },
                    { 14, "Räkor" },
                    { 15, "Musslor" },
                    { 16, "Kronärtskocka" },
                    { 17, "Oliver" },
                    { 18, "Paprika" },
                    { 19, "Spaghetti" },
                    { 20, "Köttfärssås" },
                    { 21, "Kyckling" },
                    { 22, "Curry" },
                    { 23, "Gräslök" },
                    { 24, "Krutonger" },
                    { 25, "Caesardressing" }
                });

            migrationBuilder.InsertData(
                table: "MealCategories",
                columns: new[] { "CategoryId", "CategoryName" },
                values: new object[,]
                {
                    { 1, "Pizza" },
                    { 2, "Pasta" },
                    { 3, "Sallad" }
                });

            migrationBuilder.InsertData(
                table: "Meals",
                columns: new[] { "MealId", "CategoryId", "Description", "MealName", "Price" },
                values: new object[,]
                {
                    { 1, 1, "Klassisk pizza med tomatsås och ost.", "Margherita", 8500 },
                    { 2, 1, "Pizza med tomatsås, ost och skinka.", "Vesuvio", 9000 },
                    { 3, 1, "Pizza med tomatsås, ost, skinka och ananas.", "Hawaii", 9500 },
                    { 4, 1, "Pizza med tomatsås, ost, kebabkött, lök, isbergssallad och kebabsås.", "Kebabpizza", 11000 },
                    { 5, 2, "Klassisk spaghetti med köttfärssås.", "Spaghetti Bolognese", 10500 },
                    { 6, 3, "Sallad med kyckling, romansallad, krutonger, parmesan och caesardressing.", "Caesarsallad", 11500 }
                });

            migrationBuilder.InsertData(
                table: "MealIngredients",
                columns: new[] { "IngredientId", "MealId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 1, 2 },
                    { 2, 2 },
                    { 3, 2 },
                    { 1, 3 },
                    { 2, 3 },
                    { 3, 3 },
                    { 4, 3 },
                    { 1, 4 },
                    { 2, 4 },
                    { 6, 4 },
                    { 7, 4 },
                    { 10, 4 },
                    { 12, 4 },
                    { 19, 5 },
                    { 20, 5 },
                    { 7, 6 },
                    { 21, 6 },
                    { 24, 6 },
                    { 25, 6 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MealIngredients",
                table: "MealIngredients");

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "IngredientId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "IngredientId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "IngredientId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "IngredientId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "IngredientId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "IngredientId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "IngredientId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "IngredientId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "IngredientId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "IngredientId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "IngredientId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "IngredientId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "MealIngredients",
                keyColumns: new[] { "IngredientId", "MealId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "MealIngredients",
                keyColumns: new[] { "IngredientId", "MealId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "MealIngredients",
                keyColumns: new[] { "IngredientId", "MealId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "MealIngredients",
                keyColumns: new[] { "IngredientId", "MealId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "MealIngredients",
                keyColumns: new[] { "IngredientId", "MealId" },
                keyValues: new object[] { 3, 2 });

            migrationBuilder.DeleteData(
                table: "MealIngredients",
                keyColumns: new[] { "IngredientId", "MealId" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "MealIngredients",
                keyColumns: new[] { "IngredientId", "MealId" },
                keyValues: new object[] { 2, 3 });

            migrationBuilder.DeleteData(
                table: "MealIngredients",
                keyColumns: new[] { "IngredientId", "MealId" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "MealIngredients",
                keyColumns: new[] { "IngredientId", "MealId" },
                keyValues: new object[] { 4, 3 });

            migrationBuilder.DeleteData(
                table: "MealIngredients",
                keyColumns: new[] { "IngredientId", "MealId" },
                keyValues: new object[] { 1, 4 });

            migrationBuilder.DeleteData(
                table: "MealIngredients",
                keyColumns: new[] { "IngredientId", "MealId" },
                keyValues: new object[] { 2, 4 });

            migrationBuilder.DeleteData(
                table: "MealIngredients",
                keyColumns: new[] { "IngredientId", "MealId" },
                keyValues: new object[] { 6, 4 });

            migrationBuilder.DeleteData(
                table: "MealIngredients",
                keyColumns: new[] { "IngredientId", "MealId" },
                keyValues: new object[] { 7, 4 });

            migrationBuilder.DeleteData(
                table: "MealIngredients",
                keyColumns: new[] { "IngredientId", "MealId" },
                keyValues: new object[] { 10, 4 });

            migrationBuilder.DeleteData(
                table: "MealIngredients",
                keyColumns: new[] { "IngredientId", "MealId" },
                keyValues: new object[] { 12, 4 });

            migrationBuilder.DeleteData(
                table: "MealIngredients",
                keyColumns: new[] { "IngredientId", "MealId" },
                keyValues: new object[] { 19, 5 });

            migrationBuilder.DeleteData(
                table: "MealIngredients",
                keyColumns: new[] { "IngredientId", "MealId" },
                keyValues: new object[] { 20, 5 });

            migrationBuilder.DeleteData(
                table: "MealIngredients",
                keyColumns: new[] { "IngredientId", "MealId" },
                keyValues: new object[] { 7, 6 });

            migrationBuilder.DeleteData(
                table: "MealIngredients",
                keyColumns: new[] { "IngredientId", "MealId" },
                keyValues: new object[] { 21, 6 });

            migrationBuilder.DeleteData(
                table: "MealIngredients",
                keyColumns: new[] { "IngredientId", "MealId" },
                keyValues: new object[] { 24, 6 });

            migrationBuilder.DeleteData(
                table: "MealIngredients",
                keyColumns: new[] { "IngredientId", "MealId" },
                keyValues: new object[] { 25, 6 });

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "IngredientId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "IngredientId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "IngredientId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "IngredientId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "IngredientId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "IngredientId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "IngredientId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "IngredientId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "IngredientId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "IngredientId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "IngredientId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "IngredientId",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "IngredientId",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "MealId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "MealId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "MealId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "MealId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "MealId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "MealId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "MealCategories",
                keyColumn: "CategoryId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MealCategories",
                keyColumn: "CategoryId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MealCategories",
                keyColumn: "CategoryId",
                keyValue: 3);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MealIngredients",
                table: "MealIngredients",
                column: "MealId");
        }
    }
}

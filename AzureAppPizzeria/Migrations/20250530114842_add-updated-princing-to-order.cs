using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AzureAppPizzeria.Migrations
{
    /// <inheritdoc />
    public partial class addupdatedprincingtoorder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "Orders",
                newName: "OriginalPrice");

            migrationBuilder.AddColumn<int>(
                name: "DiscountAmount",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FinalPrice",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "FinalPrice",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "OriginalPrice",
                table: "Orders",
                newName: "TotalPrice");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIronChef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangedToListInstructionsAndIngredients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Instructions",
                table: "Recipes");

            migrationBuilder.RenameColumn(
                name: "Ingredients",
                table: "Recipes",
                newName: "InstructionsJson");

            migrationBuilder.AddColumn<string>(
                name: "IngredientsJson",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IngredientsJson",
                table: "Recipes");

            migrationBuilder.RenameColumn(
                name: "InstructionsJson",
                table: "Recipes",
                newName: "Ingredients");

            migrationBuilder.AddColumn<string>(
                name: "Instructions",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

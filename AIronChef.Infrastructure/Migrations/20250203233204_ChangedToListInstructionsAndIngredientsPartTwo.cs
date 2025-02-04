using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIronChef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangedToListInstructionsAndIngredientsPartTwo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InstructionsJson",
                table: "Recipes",
                newName: "Instructions");

            migrationBuilder.RenameColumn(
                name: "IngredientsJson",
                table: "Recipes",
                newName: "Ingredients");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Instructions",
                table: "Recipes",
                newName: "InstructionsJson");

            migrationBuilder.RenameColumn(
                name: "Ingredients",
                table: "Recipes",
                newName: "IngredientsJson");
        }
    }
}

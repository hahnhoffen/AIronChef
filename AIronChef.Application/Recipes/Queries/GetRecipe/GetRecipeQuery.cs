using System.ComponentModel.DataAnnotations;

namespace AIronChef.Application.Recipes.Queries.GetRecipe
{
    public class GetRecipeQuery
    {
        [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than zero")]
        public int Id { get; set; }
    }
}

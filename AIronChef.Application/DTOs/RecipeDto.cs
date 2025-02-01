using AIronChef.Domain.Enums;

namespace AIronChef.Application.DTOs;

public class RecipeDto
{
    public ICollection<string> Ingredients { get; set; } = new List<string>();
    public int? MaxCookingTimeMinutes { get; set; }
    public MealType MealType { get; set; }
}

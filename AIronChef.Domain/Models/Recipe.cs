using AIronChef.Domain.Common;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIronChef.Domain.Models
{
    public class Recipe : IEntity
    {
        // Unique identifier for a recipe, Primary key, auto-incremented.
        public int Id { get; set; }

        // The name of the recipe, Required, max length 100.
        [Required]
        [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters.")]
        public string? Name { get; set; }

        // A brief description of the recipe, Optional, max length 500.
        [StringLength(500, ErrorMessage = "The description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        // Ingredients for the recipe as a JSON string or plain text, Required.
        //[Required]
        //public string IngredientsJson { get; set; } = "[]";
        //public string InstructionsJson { get; set; } = "[]";

        //[NotMapped]
        //public ICollection<string>? Ingredients
        //{
        //    get => JsonConvert.DeserializeObject<List<string>>(IngredientsJson) ?? new List<string>();
        //    set => IngredientsJson = JsonConvert.SerializeObject(value);
        //}
        //[NotMapped]
        //public ICollection<string>? Instructions
        //{
        //    get => JsonConvert.DeserializeObject<List<string>>(InstructionsJson) ?? new List<string>();
        //    set => InstructionsJson = JsonConvert.SerializeObject(value);
        //}

        [Required]
        public List<string>? Ingredients { get; set; }
        [Required]
        public List<string>? Instructions { get; set; }

        // The timestamp when the recipe was created.
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public int UserId { get; set; }
    }
}
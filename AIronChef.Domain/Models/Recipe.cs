using AIronChef.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace AIronChef.Domain.Models
{
    public class Recipe : IEntity
    {
        // Unique identifier for a recipe, Primary key, auto-incremented.
        public int Id { get; set; }

        // The name of the recipe, Required, max length 100.
        [Required]
        [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters.")]
        public string Name { get; set; }

        // A brief description of the recipe, Optional, max length 500.
        [StringLength(500, ErrorMessage = "The description cannot exceed 500 characters.")]
        public string Description { get; set; }

        // Ingredients for the recipe as a JSON string or plain text, Required.
        [Required]
        public string Ingredients { get; set; }

        // The timestamp when the recipe was created.
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
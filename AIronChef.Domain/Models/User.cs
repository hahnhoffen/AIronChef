using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIronChef.Domain.Models
{
    public class User
    {
        // Primary key, auto-incremented by the database.
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // User's full name.
        [Required]
        public required string Name { get; set; }

        // User's email address.
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        // Hashed version of the user's password.
        public string PasswordHash { get; set; } = string.Empty;

        // Salt used to hash the password so it becomes unique even if the same pasword is used by 2 different people or reused by the same person.
        public string PasswordSalt { get; set; } = string.Empty;
    }
}

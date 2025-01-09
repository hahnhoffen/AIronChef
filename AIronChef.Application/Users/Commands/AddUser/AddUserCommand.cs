using System.ComponentModel.DataAnnotations;

namespace AIronChef.Application.Users.Commands.AddUser;

public class AddUserCommand
{
    /// <summary>
    /// Gets or sets the full name of the user.
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [StringLength(40, ErrorMessage = "Name cannot be longer than 40 characters")]
    public string Name { get; set; }
    
    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is invalid format.")]
    public string Email { get; set; }
    
    /// <summary>
    /// Gets or sets the plain text password of the user.
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    [StringLength(22, ErrorMessage = "Password cannot be longer than 22 characters")]
    public string Password { get; set; }
}
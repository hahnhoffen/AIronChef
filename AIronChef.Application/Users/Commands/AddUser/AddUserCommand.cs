using System.ComponentModel.DataAnnotations;

namespace AIronChef.Application.Users.Commands.AddUser;

public class AddUserCommand
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
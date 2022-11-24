using System.ComponentModel.DataAnnotations;

namespace BackendTest.Models;

public class UserRegister
{
    [Required]
    public string Name { get; set; }
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }

    public UserRegister(string name, string email, string password)
    {
        Name = name;
        Email = email;
        Password = password;
    }
}
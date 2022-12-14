using System.ComponentModel.DataAnnotations;

namespace BackendTest.Models;

public class UserLogin
{
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}
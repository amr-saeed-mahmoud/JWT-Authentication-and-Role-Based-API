using System.ComponentModel.DataAnnotations;

namespace Test.DTOs;

public class SignInDto
{
    [Required]
    public string? UserName { get; set; }
    
    [Required]
    public string? Email { get; set; }

    [Required]
    public string? Password { get; set; }

    public string? PhoneNumber { get; set; }

    [Required]
    public string? Role { get; set; }
}
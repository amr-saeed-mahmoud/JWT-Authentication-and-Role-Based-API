using System.ComponentModel.DataAnnotations;

namespace Test.DTOs;

public class LogInDto
{
    [Required]
    public string? UserName { get; set; }

    [Required]
    public string? Password { get; set; }
}
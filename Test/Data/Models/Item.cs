using System.ComponentModel.DataAnnotations;

namespace Test.Data.Models;

public class Item
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(30)]
    public string? Name { get; set; }

    [Required]
    public decimal Price { get; set; }
}
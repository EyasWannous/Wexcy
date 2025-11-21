using System.ComponentModel.DataAnnotations;

namespace Wexcy.Application.Products.DTOs;

public class UpdateProductRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required]
    public string ConcurrencyStamp { get; set; } = string.Empty;
}
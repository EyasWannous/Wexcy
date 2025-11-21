namespace Wexcy.Application.Products.DTOs;

public class GetProductsQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Keyword { get; set; }
    public string? Category { get; set; }
    public bool? IncludeDeleted { get; set; } = false;
}

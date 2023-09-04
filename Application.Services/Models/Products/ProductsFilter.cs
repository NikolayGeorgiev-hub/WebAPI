namespace Application.Services.Models.Products;

public class ProductsFilter : PaginationRequestModel
{
    public Guid? CategoryId { get; set; }

    public string? SearchTerm { get; set; }

    public IReadOnlyList<Guid>? SubCategories { get; set; }
}

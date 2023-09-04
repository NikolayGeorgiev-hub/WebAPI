using Application.Data.Models.Categories;

namespace Application.Data.Models.Products;

public class Product
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public bool InStock { get; set; }

    public Guid CategoryId { get; set; }

    public Category Category { get; set; }

    public Guid SubCategoryId { get; set; }

    public SubCategory SubCategory { get; set; }

    public Guid OwnerId { get; set; }
}

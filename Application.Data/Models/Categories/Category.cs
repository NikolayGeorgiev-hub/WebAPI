using Application.Data.Models.Products;

namespace Application.Data.Models.Categories;

public class Category
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public ICollection<Product> Products { get; set; }

    public ICollection<SubCategory> SubCategories { get; set; }
}

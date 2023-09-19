using Application.Data.Models.Categories;
using Application.Data.Models.Comments;
using Application.Data.Models.Discounts;
using Application.Data.Models.Orders;
using Application.Data.Models.Ratings;

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

    public ICollection<Rating> Ratings { get; set; }

    public ICollection<ProductsList> Orders { get; set; }

    public ICollection<Comment> Comments { get; set; }

    public decimal? NewPrice { get; set; }

    public decimal? Percentage { get; set; }

    public Guid? DiscountId { get; set; }
}


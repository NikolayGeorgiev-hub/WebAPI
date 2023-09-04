using Application.Data.Models.Products;

namespace Application.Data.Models.Ratings;

public class Rating
{
    public Guid Id { get; set; }

    public DateTime CreatedOn { get; set; }

    public int Value { get; set; }

    public Guid ProductId { get; set; }

    public Guid UserId { get; set; }
}

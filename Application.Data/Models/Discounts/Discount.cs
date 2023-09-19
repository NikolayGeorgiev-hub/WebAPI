using Application.Data.Models.Products;

namespace Application.Data.Models.Discounts;

public class Discount
{
    public Guid Id { get; set; }

    public string Description { get; set; }

    public decimal Percentage { get; set; }

    public bool IsActive { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? JodId { get; set; }
}

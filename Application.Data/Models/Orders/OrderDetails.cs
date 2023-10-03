namespace Application.Data.Models.Orders;

public class OrderDetails
{
    public int Id { get; set; }

    public Guid OrderId { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal? TotalAmountWithDiscount { get; set; }

    public decimal? Difference { get; set; }

    public ICollection<OrderProductDetails> Products { get; set; }
}

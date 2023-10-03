namespace Application.Data.Models.Orders;

public class OrderProductDetails
{
    public int Id { get; set; }

    public string ProductName { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal? PriceWithDiscount { get; set; }

    public int OrderDetailsId { get; set; }
}

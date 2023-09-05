using Application.Data.Models.Products;

namespace Application.Data.Models.Orders;

public class ProductsList
{
    public Guid ProductId { get; set; }

    public Product Product { get; set; }

    public Guid OrderId { get; set; }

    public Order Order { get; set; }

    public int Quantity { get; set; }
}

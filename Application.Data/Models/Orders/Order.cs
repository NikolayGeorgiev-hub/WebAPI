namespace Application.Data.Models.Orders;

public class Order
{
    public Order()
    {
        this.Products = new List<ProductsList>();
    }

    public Guid Id { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid UserId { get; set; }

    public OrderStatus Status { get; set; }

    public ICollection<ProductsList> Products { get; set; }

    public int? DetailsId { get; set; }

    public OrderDetails? Details { get; set; }
}

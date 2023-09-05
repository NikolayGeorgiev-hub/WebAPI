namespace Application.Data.Models.Orders;

public class Order
{
    public Guid Id { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid UserId { get; set; }

    public OrderStatus Status { get; set; }

    public ICollection<ProductsList> Products { get; set; }
}

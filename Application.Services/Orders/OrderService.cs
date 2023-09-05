using Application.Data;

namespace Application.Services.Orders;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext dbContext;

    public OrderService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
}

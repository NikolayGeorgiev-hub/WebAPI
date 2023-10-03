using Application.Data.Models.Orders;
using Application.Data.Models.Products;
using Microsoft.EntityFrameworkCore;

namespace Application.Data.Repositories.Orders;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext dbContext;

    public OrderRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(Order order)
    {
        await this.dbContext.Orders.AddAsync(order);
    }

    public async Task AddOrderDetailsAsync(OrderDetails orderDetails)
    {
        await this.dbContext.OrderDetails.AddAsync(orderDetails);
    }

    public async Task AddProductToOrderAsync(ProductsList productsList)
    {
        await this.dbContext.ProductsLists.AddAsync(productsList);
    }

    public async Task<IReadOnlyList<Order>> GetOrderHistoryAsync(Guid userId)
    {
        IReadOnlyList<Order> orders = await this.dbContext.Orders
            .Where(x => x.UserId == userId && x.Status != OrderStatus.InProgress)
            .Include(x => x.Details)
            .ThenInclude(x => x.Products)
            .ToListAsync();

        return orders;
    }

    public async Task<Order?> GetUserOrderInProgressAsync(Guid userId)
    {
        Order? order = await this.dbContext.Orders
            .Where(x => x.Products.Count > 0 && x.Status == OrderStatus.InProgress)
            .Include(x => x.Products)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.UserId == userId);

        return order;
    }

    public async Task RemoveProductWhenOutOfStockAsync(Guid productId)
    {
        IQueryable<Order> orderQuery = this.dbContext.Orders
            .Include(x => x.Products)
            .Where(order
                => order.Products
                    .Select(x => x.ProductId)
                    .Contains(productId) && order.Status == OrderStatus.InProgress);

        await orderQuery.ForEachAsync(async order =>
        {
            IList<ProductsList> productsInOrder = order.Products.Where(x => x.ProductId == productId).ToList();
            this.dbContext.ProductsLists.RemoveRange(productsInOrder);
        });

    }

    public void RemoveProductsFromOrder(ProductsList productInOrder)
    {
        this.dbContext.ProductsLists.Remove(productInOrder);
    }

    public async Task SaveChangesAsync()
    {
        await this.dbContext.SaveChangesAsync();
    }
}

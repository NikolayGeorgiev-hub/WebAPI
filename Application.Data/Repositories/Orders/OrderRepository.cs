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

    public async Task SaveChangesAsync()
    {
        await this.dbContext.SaveChangesAsync();
    }
}

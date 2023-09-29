using Application.Data.Models.Orders;
using System.Threading.Tasks;

namespace Application.Data.Repositories.Orders;

public interface IOrderRepository
{
    Task AddAsync(Order order);

    Task AddProductToOrderAsync(ProductsList productsList);

    Task<Order?> GetUserOrderInProgressAsync(Guid userId);

    Task RemoveProductWhenOutOfStockAsync(Guid productId);

   void RemoveProductsFromOrder(ProductsList productInOrder);

    Task SaveChangesAsync();
}

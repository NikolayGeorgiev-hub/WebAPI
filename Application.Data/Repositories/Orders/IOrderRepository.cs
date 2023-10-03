using Application.Data.Models.Orders;
using System.Threading.Tasks;

namespace Application.Data.Repositories.Orders;

public interface IOrderRepository
{
    Task AddAsync(Order order);

    Task AddOrderDetailsAsync(OrderDetails orderDetails);

    Task AddProductToOrderAsync(ProductsList productsList);

    Task<IReadOnlyList<Order>> GetOrderHistoryAsync(Guid userId);

    Task<Order?> GetUserOrderInProgressAsync(Guid userId);

    Task RemoveProductWhenOutOfStockAsync(Guid productId);

   void RemoveProductsFromOrder(ProductsList productInOrder);

    Task SaveChangesAsync();
}

using Application.Services.Models.Orders;

namespace Application.Services.Orders;

public interface IOrderService
{
    Task AddProductAsync(Guid userId, Guid productId);

    Task EditProductsQuantityAsync(Guid userId, Guid productId, string action);

    Task RemoveProductAsync(Guid userId, Guid productId);

    Task<IReadOnlyList<OrderResponseModel>> OrdersHistoryAsync(Guid userId);

    Task<OrderDetailsResponseModel> OrderDetailsAsync(Guid userId);

    Task<OrderDetailsResponseModel> SendOrderAsync(Guid userId);

    Task CancelOrderAsync(Guid userId, Guid orderId);
}

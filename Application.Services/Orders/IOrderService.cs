using Application.Services.Models.Orders;

namespace Application.Services.Orders;

public interface IOrderService
{
    Task AddProductAsync(Guid userId, Guid productId);

    Task EditProductsQuantityAsync(Guid userId, Guid productId, string action);

    Task RemoveProductAsync(Guid userId, Guid productId);

    Task<OrderDetailsResponseModel> OrderDetailsAsync(Guid userId);
}

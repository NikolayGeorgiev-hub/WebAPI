using System.Threading.Tasks;

namespace Application.Data.Repositories.Orders;

public interface IOrderRepository
{
    Task RemoveProductWhenOutOfStockAsync(Guid productId);
}

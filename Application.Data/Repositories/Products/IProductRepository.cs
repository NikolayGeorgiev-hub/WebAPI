using Application.Common.Models;
using Application.Data.Models.Products;
using System.Threading.Tasks;

namespace Application.Data.Repositories.Products;

public interface IProductRepository
{
    Task AddAsync(Product product);

    Task<IReadOnlyList<Product>> GetAllAsync(ProductsFilter productsFilter);

    Task<Product?> GetByIdAsync(Guid id);

    Task<int> GetCountAsync(ProductsFilter productsFilter);

    Task ExistsProductNameAsync(string productName);

    Task ExistsProductNameWhenUpdateAsync(string productName, Guid productId);

    Task<bool> ExistsProductByIdAsync(Guid productId);

    Task<bool> ExistsProductInStockAsync(Guid productId);

    Task SaveChangesAsync();
}

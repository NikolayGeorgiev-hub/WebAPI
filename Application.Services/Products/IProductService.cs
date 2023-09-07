using Application.Services.Models;
using Application.Services.Models.Products;

namespace Application.Services.Products;

public interface IProductService
{
    Task CreteProductAsync(Guid ownerId, CreateProductRequestModel requestModel);

    Task<PaginationResponseModel<ProductResponseModel>> GetAllProductsAsync(ProductsFilter productsFilter);

    Task<ProductDetailsResponseModel> GetProductDetailsAsync(Guid productId, PaginationRequestModel requestModel);
}

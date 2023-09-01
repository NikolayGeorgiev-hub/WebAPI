using Application.Services.Models.Products;

namespace Application.Services.Products;

public interface IProductService
{
    Task CreteProductAsync(Guid ownerId, CreateProductRequestModel requestModel);
}

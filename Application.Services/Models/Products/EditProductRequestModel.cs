namespace Application.Services.Models.Products;

public record EditProductRequestModel(string Name, string Description, decimal Price, int Quantity, Guid CategoryId, Guid SubCategoryId, bool InStock);



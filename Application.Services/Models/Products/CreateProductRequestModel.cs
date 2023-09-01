namespace Application.Services.Models.Products;

public record CreateProductRequestModel(string Name, string Description, decimal Price, Guid CategoryId, Guid SubCategoryId);



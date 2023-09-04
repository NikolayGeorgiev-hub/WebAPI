namespace Application.Services.Models.Products;

public record ProductResponseModel(string Name, string Description, decimal Price, int Quantity, string CategoryName, string SubCategoryName, bool InStock);



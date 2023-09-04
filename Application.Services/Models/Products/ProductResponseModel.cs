using Application.Services.Models.Ratings;
using Application.Services.Validators.Ratings;

namespace Application.Services.Models.Products;

public record ProductResponseModel(string Name, string Description, decimal Price, int Quantity, string CategoryName, string SubCategoryName, bool InStock, RatingResponseModel RatingResponse);



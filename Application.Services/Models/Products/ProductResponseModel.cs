using Application.Services.Models.Ratings;

namespace Application.Services.Models.Products;

public record ProductResponseModel(
    string Name,
    string Description,
    decimal Price,
    decimal? DiscountValue,
    decimal? NewPrice,
    int Quantity,
    string CategoryName,
    string SubCategoryName,
    bool InStock,
    RatingResponseModel RatingResponse);



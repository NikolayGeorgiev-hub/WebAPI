using Application.Common.Models;
using Application.Services.Models.Comments;
using Application.Services.Models.Ratings;

namespace Application.Services.Models.Products;

public record ProductDetailsResponseModel(
    string Name,
    string Description,
    decimal Price,
    int Quantity,
    string CategoryName,
    string SubCategoryName,
    bool InStock,
    RatingResponseModel RatingResponse,
    PaginationResponseModel<CommentResponseModel> comments);



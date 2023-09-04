using Application.Data.Models.Products;
using Application.Data.Models.Users;
using Application.Services.Models.Products;
using Application.Services.Models.Ratings;
using Application.Services.Models.Users;

namespace Application.Services.Extensions;

public static class AutoMapperExtensions
{
    public static UserResponseModels.Profile ToUserProfile(this ApplicationUser user)
        => new UserResponseModels.Profile(user.FirstName, user.Email!, user.PhoneNumber ?? "n/a");

    public static ProductResponseModel ToProductResponseModel(this Product product, RatingResponseModel ratingResponse)
        => new ProductResponseModel(
            product.Name,
            product.Description,
            product.Price,
            product.Quantity,
            product.Category.Name,
            product.SubCategory.Name,
            product.InStock,
            ratingResponse);
}

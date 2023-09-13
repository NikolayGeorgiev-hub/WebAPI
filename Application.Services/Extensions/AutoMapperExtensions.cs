using Application.Data.Models.Orders;
using Application.Data.Models.Products;
using Application.Data.Models.Users;
using Application.Services.Models;
using Application.Services.Models.Comments;
using Application.Services.Models.Orders;
using Application.Services.Models.Products;
using Application.Services.Models.Ratings;
using Application.Services.Models.Users;

namespace Application.Services.Extensions;

public static class AutoMapperExtensions
{
    public static UserResponseModels.Profile ToUserProfile(this ApplicationUser user)
        => new UserResponseModels.Profile(
            user.FirstName,
            user.Email!,
            user.PhoneNumber ?? "n/a");

    public static ProductResponseModel ToProductResponseModel(this Product product, RatingResponseModel ratingResponse)
        => new ProductResponseModel(
            product.Name,
            product.Description,
            product.Price,
            product.DiscountValue,
            product.NewPrice,
            product.Quantity,
            product.Category.Name,
            product.SubCategory.Name,
            product.InStock,
            ratingResponse);

    public static ProductDetailsResponseModel ToProductDetailsResponseModel(this Product product, RatingResponseModel ratingResponse, PaginationResponseModel<CommentResponseModel> commentsResponse)
        => new ProductDetailsResponseModel(
            product.Name,
            product.Description,
            product.Price,
            product.DiscountValue,
            product.NewPrice,
            product.Quantity,
            product.Category.Name,
            product.SubCategory.Name,
            product.InStock,
            ratingResponse,
            commentsResponse);


    public static ProductInOrderResponseModel ToProductInOrder(this ProductsList item)
        => new ProductInOrderResponseModel(
            item.Product.Name,
            item.Product.Price,
            item.Product.DiscountValue,
            item.Product.NewPrice,
            item.Quantity,
            item.Quantity * item.Product.NewPrice ?? item.Quantity * item.Product.Price);

    public static OrderDetailsResponseModel ToOrderDetails(this Order order, IReadOnlyList<ProductInOrderResponseModel> products)
        => new OrderDetailsResponseModel(
            order.CreatedOn,
            order.Status.ToString(),
            products.Select(details => details.TotalPrice).Sum(),
            products.Select(details => details.Price * details.Quantity).Sum() - products.Select(details => details.TotalPrice).Sum(),
            products);

}

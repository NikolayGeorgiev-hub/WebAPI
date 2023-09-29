using Application.Common.Models;
using Application.Data.Models.Orders;
using Application.Data.Models.Products;
using Application.Data.Models.Users;
using Application.Services.Models;
using Application.Services.Models.Comments;
using Application.Services.Models.Orders;
using Application.Services.Models.Products;
using Application.Services.Models.Ratings;
using Application.Services.Models.Users;
using Microsoft.EntityFrameworkCore.Infrastructure;

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
            product.Quantity,
            product.Category.Name,
            product.SubCategory.Name,
            product.InStock,
            ratingResponse,
            commentsResponse);

    public static ProductInOrderResponseModel ToProductInOrderInfo(this ProductsList item)
        => new ProductInOrderResponseModel(
            item.Product.Name,
            item.Product.Price,
            item.Quantity,
            TotalPrice: item.Product.Price * item.Quantity,
            TotalPriceDiscount: item.Product.DiscountId is not null ? item.Product.NewPrice * item.Quantity : null);

    public static OrderDetailsResponseModel ToOrderDetails(
        this Order order,
        decimal totalPrice,
        decimal totalPriceDiscount,
        decimal difference,
        IReadOnlyList<ProductInOrderResponseModel> products)
        => new OrderDetailsResponseModel(
            order.CreatedOn,
            order.Status.ToString(),
            totalPrice,
            totalPriceDiscount,
            difference,
            products);
}

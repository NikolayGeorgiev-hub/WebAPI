namespace Application.Services.Models.Orders;

public record OrderDetailsResponseModel(
    DateTime CreatedOn, 
    string Status, 
    decimal TotalPrice, 
    decimal? DiscountValue,
    IReadOnlyList<ProductInOrderResponseModel> Products);



namespace Application.Services.Models.Orders;

public record OrderDetailsResponseModel(
    DateTime CreatedOn, 
    string Status, 
    decimal TotalPrice, 
    decimal TotalPriceDiscount,
    decimal Difference,
    IReadOnlyList<ProductInOrderResponseModel> Products);



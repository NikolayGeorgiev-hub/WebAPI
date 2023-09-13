namespace Application.Services.Models.Orders;

public record ProductInOrderResponseModel(
    string ProductName,
    decimal Price,
    decimal? DiscountValue,
    decimal? NewPrice,
    int Quantity,
    decimal TotalPrice);




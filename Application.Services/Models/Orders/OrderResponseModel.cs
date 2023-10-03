namespace Application.Services.Models.Orders;

public record OrderResponseModel(
    DateTime CreatedOn,
    string OrderStatus,
    decimal TotalAmount,
    decimal? TotalAmountWithDiscount,
    decimal? Difference,
    IReadOnlyList<ProductInOrderResponseModel> Products);



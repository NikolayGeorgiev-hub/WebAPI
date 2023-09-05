namespace Application.Services.Models.Orders;

public record OrderDetailsResponseModel(DateTime CreatedOn, string Status, decimal TotalPrice, IReadOnlyList<ProductInOrderResponseModel> Products);



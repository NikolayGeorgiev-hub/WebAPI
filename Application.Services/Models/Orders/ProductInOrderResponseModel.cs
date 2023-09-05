namespace Application.Services.Models.Orders;

public record ProductInOrderResponseModel(string ProductName, decimal Price, int Quantity, decimal TotalPrice);




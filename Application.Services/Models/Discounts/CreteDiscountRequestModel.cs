namespace Application.Services.Models.Discounts;

public record CreteDiscountRequestModel(string Description, decimal Percentage, Guid? CategoryId, Guid? SubCategoryId);



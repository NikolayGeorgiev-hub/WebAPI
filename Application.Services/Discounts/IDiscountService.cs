using Application.Services.Models.Discounts;

namespace Application.Services.Discounts;

public interface IDiscountService
{
    Task CreteDiscountAsync(CreteDiscountRequestModel requestModel);
}

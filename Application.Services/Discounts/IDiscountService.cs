using Application.Services.Models.Discounts;

namespace Application.Services.Discounts;

public interface IDiscountService
{
    Task CreateDiscountAsync(CreteDiscountRequestModel requestModel);

    Task RemoveDiscountAsync(Guid discountId);
}

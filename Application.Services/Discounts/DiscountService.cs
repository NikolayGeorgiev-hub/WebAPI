using Application.Data;
using Application.Services.Models.Discounts;

namespace Application.Services.Discounts;

public class DiscountService : IDiscountService
{
    private readonly ApplicationDbContext dbContext;

    public DiscountService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task CreateDiscountAsync(CreteDiscountRequestModel requestModel)
    {

    }
}

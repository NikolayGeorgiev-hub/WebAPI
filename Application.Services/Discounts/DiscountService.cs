using Application.Common.Exceptions.Discounts;
using Application.Data;
using Application.Data.Models.Discounts;
using Application.Data.Models.Products;
using Application.Services.Models.Discounts;
using Microsoft.EntityFrameworkCore;

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
        Discount discount = new()
        {
            IsActive = true,
            Description = requestModel.Description,
            Percentage = requestModel.Percentage,
        };

        await this.dbContext.Discounts.AddAsync(discount);

        if (requestModel.CategoryId is not null)
        {
            await this.dbContext.Products
                .Where(x => x.CategoryId == requestModel.CategoryId && x.InStock == true)
                .ForEachAsync(product =>
                {
                    product.DiscountId = discount.Id;
                    product.NewPrice = product.Price - product.Price * discount.Percentage / 100;
                    product.DiscountValue = discount.Percentage;
                });
        }

        await this.dbContext.SaveChangesAsync();
    }

    public async Task RemoveDiscountAsync(Guid discountId)
    {
        Discount? discount = await this.dbContext.Discounts.FirstOrDefaultAsync(x => x.Id == discountId);
        if (discount is null)
        {
            throw new NotFoundDiscountException("Invalid discount");
        }

        await this.dbContext.Products
            .Where(x => x.DiscountId == discountId && x.InStock == true)
            .ForEachAsync(product =>
            {
                product.DiscountId = null;
                product.NewPrice = null;
                product.DiscountValue = null;

            });

        this.dbContext.Discounts.Remove(discount);
        await this.dbContext.SaveChangesAsync();
    }
}

using Application.Common.Exceptions.Discounts;
using Application.Common.Exceptions.Products;
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
            Description = requestModel.Description,
            Percentage = requestModel.Percentage,
        };

        if (requestModel.StartDate is not null && requestModel.EndDate is not null)
        {
            discount.IsActive = false;
        }
        else
        {
            discount.IsActive = true;
        }

        await this.dbContext.Discounts.AddAsync(discount);

        if (discount.IsActive)
        {
            if (requestModel.CategoryId is not null)
            {
                bool existsCategory = await this.dbContext.Categories.AnyAsync(x => x.Id == requestModel.CategoryId);
                if (!existsCategory)
                {
                    throw new NotFoundCategoryException("Invalid category");
                }

                await this.dbContext.Products
                    .Where(x => x.CategoryId == requestModel.CategoryId && x.InStock == true)
                    .ForEachAsync(product => { UpdateProduct(product, discount); });

                await this.dbContext.SaveChangesAsync();
                return;

            }

            if (requestModel.SubCategoryId is not null)
            {
                bool existsSubCategory = await this.dbContext.SubCategories.AnyAsync(x => x.Id == requestModel.SubCategoryId);
                if (!existsSubCategory)
                {
                    throw new NotFoundCategoryException("Invalid category");
                }

                await this.dbContext.Products
                    .Where(x => x.SubCategoryId == requestModel.SubCategoryId && x.InStock == true)
                    .ForEachAsync(product => { UpdateProduct(product, discount); });

                await this.dbContext.SaveChangesAsync();
                return;
            }

            if (requestModel.Products is not null)
            {
                foreach (var productId in requestModel.Products)
                {
                    Product? product = await this.dbContext.Products.FirstOrDefaultAsync(x => x.Id == productId);
                    if (product is not null)
                    {
                        UpdateProduct(product, discount);
                    }
                }

                await this.dbContext.SaveChangesAsync();
                return;
            }
        }
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

    private void UpdateProduct(Product product, Discount discount)
    {
        product.DiscountId = discount.Id;
        product.DiscountValue = discount.Percentage;
        product.NewPrice = product.Price - product.Price * discount.Percentage / 100;
    }
}

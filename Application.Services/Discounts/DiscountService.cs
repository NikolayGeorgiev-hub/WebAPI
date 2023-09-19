using Application.Common.Exceptions.Discounts;
using Application.Common.Exceptions.Products;
using Application.Data;
using Application.Data.Models.Discounts;
using Application.Data.Models.Products;
using Application.Services.Models.Discounts;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using Hangfire.States;

namespace Application.Services.Discounts;

public class DiscountService : IDiscountService
{
    private readonly ApplicationDbContext dbContext;
    private readonly IBackgroundJobClient backgroundJob;

    public DiscountService(ApplicationDbContext dbContext, IBackgroundJobClient backgroundJob)
    {
        this.dbContext = dbContext;
        this.backgroundJob = backgroundJob;
    }

    public async Task CreateDiscountAsync(CreteDiscountRequestModel requestModel)
    {
        Discount discount = new()
        {
            Description = requestModel.Description,
            Percentage = requestModel.Percentage,
            IsActive = false,
        };

        await this.dbContext.Discounts.AddAsync(discount);


        if (requestModel.StartDate is not null && requestModel.EndDate is not null)
        {
            TimeSpan discountStart = requestModel.StartDate.Value - DateTime.Now;
            TimeSpan discountEnd = requestModel.EndDate.Value - requestModel.StartDate.Value;

            if (requestModel.CategoryId is not null)
            {
                bool existsCategory = await this.dbContext.Categories.AnyAsync(x => x.Id == requestModel.CategoryId);
                if (!existsCategory)
                {
                    throw new NotFoundCategoryException("Invalid category");
                }

                string jobId = this.backgroundJob.Schedule(()
                    => this.ApplyDiscountCategoryAsync(requestModel.CategoryId.Value, discount), TimeSpan.FromMinutes(discountStart.TotalMinutes));

                string removeDiscountJobId = this.backgroundJob.Schedule(()
                      => this.RemoveDiscountAsync(discount.Id), TimeSpan.FromMinutes(discountEnd.TotalMinutes));

                discount.JodId = jobId;
            }

            if (requestModel.SubCategoryId is not null)
            {
                bool existsSubCategory = await this.dbContext.SubCategories.AnyAsync(x => x.Id == requestModel.SubCategoryId);
                if (!existsSubCategory)
                {
                    throw new NotFoundCategoryException("Invalid category");
                }

                string jobId = this.backgroundJob.Schedule(()
                    => this.ApplyDiscountSybCategoryAsync(requestModel.SubCategoryId.Value, discount), TimeSpan.FromMinutes(1));

                discount.JodId = jobId;
            }

            if (requestModel.Products is not null)
            {
                string jobId = this.backgroundJob.Schedule(()
                    => ApplyDiscountSelectedProductsAsync(requestModel.Products, discount), TimeSpan.FromMinutes(1));

                discount.JodId = jobId;
            }

            discount.StartDate = requestModel.StartDate;
            discount.EndDate = requestModel.EndDate;

        }
        else
        {
            discount.IsActive = true;
        }

        if (discount.IsActive)
        {
            if (requestModel.CategoryId is not null)
            {
                bool existsCategory = await this.dbContext.Categories.AnyAsync(x => x.Id == requestModel.CategoryId);
                if (!existsCategory)
                {
                    throw new NotFoundCategoryException("Invalid category");
                }

                await this.ApplyDiscountCategoryAsync(requestModel.CategoryId.Value, discount);

            }

            if (requestModel.SubCategoryId is not null)
            {
                bool existsSubCategory = await this.dbContext.SubCategories.AnyAsync(x => x.Id == requestModel.SubCategoryId);
                if (!existsSubCategory)
                {
                    throw new NotFoundCategoryException("Invalid category");
                }

                await this.ApplyDiscountSybCategoryAsync(requestModel.SubCategoryId.Value, discount);
            }

            if (requestModel.Products is not null)
            {
                await this.ApplyDiscountSelectedProductsAsync(requestModel.Products, discount);
            }
        }

        await this.dbContext.SaveChangesAsync();
    }

    public async Task ApplyDiscountCategoryAsync(Guid categoryId, Discount discount)
    {
        await this.dbContext.Products
            .Where(x => x.CategoryId == categoryId && x.InStock == true)
            .ForEachAsync(product => { UpdateProduct(product, discount); });

        await this.dbContext.SaveChangesAsync();
        return;
    }

    public async Task ApplyDiscountSybCategoryAsync(Guid subCategoryId, Discount discount)
    {
        await this.dbContext.Products
            .Where(x => x.SubCategoryId == subCategoryId && x.InStock == true)
            .ForEachAsync(product => { UpdateProduct(product, discount); });

        await this.dbContext.SaveChangesAsync();
        return;
    }

    public async Task ApplyDiscountSelectedProductsAsync(IReadOnlyList<Guid> products, Discount discount)
    {
        foreach (var productId in products)
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

    public async Task RemoveDiscountAsync(Guid discountId)
    {
        Discount? discount = await this.dbContext.Discounts.FirstOrDefaultAsync(x => x.Id == discountId);
        if (discount is null)
        {
            throw new NotFoundDiscountException("Invalid discount");
        }

        if (discount.JodId is not null)
        {
            this.backgroundJob.Delete(discount.JodId);
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

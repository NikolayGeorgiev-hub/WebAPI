using Application.Common.Exceptions.Products;
using Application.Data;
using Application.Data.Models.Discounts;
using Application.Data.Models.Products;
using Application.Services.Models.Discounts;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;

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

    public async Task CreteDiscountAsync(CreteDiscountRequestModel requestModel)
    {
        if (requestModel.StartDate is not null && requestModel.EndDate is not null)
        {
            TimeSpan startDiscount = requestModel.StartDate.Value - DateTime.Now;
            TimeSpan endDiscount = requestModel.EndDate.Value - requestModel.StartDate.Value;

            Discount discount = new()
            {
                Description = requestModel.Description,
                Percentage = requestModel.Percentage,
                StartDate = requestModel.StartDate,
                EndDate = requestModel.EndDate,
            };

            await this.dbContext.Discounts.AddAsync(discount);

            string startJobId = this.backgroundJob.Schedule(()
                => Console.WriteLine($"Start discount After: {startDiscount.Days} days : {startDiscount.Hours} hours : {startDiscount.Minutes} minutes"), TimeSpan.FromMinutes(startDiscount.TotalMinutes));

            string endJobId = this.backgroundJob.Schedule(()
                => Console.WriteLine($"End discount After: {startDiscount.Days} days : {startDiscount.Hours} hours : {startDiscount.Minutes} minutes)"), TimeSpan.FromMinutes(endDiscount.TotalMinutes));

            await this.dbContext.SaveChangesAsync();
        }
        else
        {
            Discount discount = new()
            {
                Description = requestModel.Description,
                Percentage = requestModel.Percentage,
            };

            await this.dbContext.Discounts.AddAsync(discount);
            await this.dbContext.SaveChangesAsync();

            if (requestModel.CategoryId is not null)
            {
                await this.ApplyCategoryDiscountAsync(requestModel.CategoryId!.Value, discount);
            }

            if (requestModel.SubCategoryId is not null)
            {
                await this.ApplySubCategoryDiscountAsync(requestModel.SubCategoryId!.Value, discount);
            }
        }
    }

    public async Task ApplyCategoryDiscountAsync(Guid categoryId, Discount discount)
    {
        bool existsCategory = await this.dbContext.Categories.AnyAsync(x => x.Id == categoryId);
        if (!existsCategory)
        {
            throw new NotFoundCategoryException("Invalid category");
        }

        IQueryable<Product> productsQuery = this.dbContext.Products.Where(x => x.CategoryId == categoryId);

        await this.UpdateProductsAsync(productsQuery, discount);
        await this.dbContext.SaveChangesAsync();
    }

    public async Task ApplySubCategoryDiscountAsync(Guid subCategoryId, Discount discount)
    {
        bool existsSubCategory = await this.dbContext.SubCategories.AnyAsync(x => x.Id == subCategoryId);
        if (!existsSubCategory)
        {
            throw new NotFoundCategoryException("Invalid category");
        }

        IQueryable<Product> productsQuery = this.dbContext.Products.Where(x => x.Id == subCategoryId);

        await this.UpdateProductsAsync(productsQuery, discount);
        await this.dbContext.SaveChangesAsync();
    }


    private async Task UpdateProductsAsync(IQueryable<Product> productsQuery, Discount discount)
    {
        await productsQuery.ForEachAsync(product =>
        {
            product.DiscountId = discount.Id;
            product.Percentage = discount.Percentage;
            product.NewPrice = product.Price - product.Price * discount.Percentage / 100;
        });
    }
}

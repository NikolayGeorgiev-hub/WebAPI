using Application.Common.Exceptions.Discounts;
using Application.Common.Exceptions.Products;
using Application.Data;
using Application.Data.Models.Categories;
using Application.Data.Models.Discounts;
using Application.Data.Models.Products;
using Application.Services.Models.Discounts;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace Application.Services.Discounts;

public class DiscountService : IDiscountService
{
    private readonly ApplicationDbContext dbContext;
    private readonly IBackgroundJobClient backgroundJob;
    private readonly ILogger<DiscountService> logger;

    public DiscountService(ApplicationDbContext dbContext, IBackgroundJobClient backgroundJob, ILogger<DiscountService> logger)
    {
        this.dbContext = dbContext;
        this.backgroundJob = backgroundJob;
        this.logger = logger;
    }


    public async Task CreteDiscountAsync(CreteDiscountRequestModel requestModel)
    {
        bool isActiveDiscount = await this.dbContext.Discounts.AnyAsync(discount => discount.IsActive);
        if (isActiveDiscount)
            throw new ActiveDiscountException("Now have active discount first remove this discount and after create new");

        Discount discount = new()
        {
            Description = requestModel.Description,
            Percentage = requestModel.Percentage,
        };

        this.dbContext.Discounts.Add(discount);

        if (requestModel.StartDate is not null && requestModel.EndDate is not null)
        {
            TimeSpan startDiscountDate = requestModel.StartDate.Value - DateTime.Now;
            TimeSpan endDiscountDate = requestModel.EndDate.Value - requestModel.StartDate.Value;

            discount.StartDate = requestModel.StartDate.Value;
            discount.EndDate = requestModel.EndDate.Value;

            string startJobId = this.backgroundJob.Schedule(()
                => this.ApplyDiscountAsync(requestModel, discount), TimeSpan.FromSeconds(/*startDiscount.TotalMinutes*/ 10));

            string endJobId = this.backgroundJob.Schedule(()
                => this.RemoveProductDiscountAsync(discount), TimeSpan.FromMinutes(/*endDiscount.TotalMinutes*/ 30));

            discount.StartBackgroundJodId = startJobId;
            discount.EndBackGroundJodId = endJobId;
        }

        if (requestModel.StartDate is null && requestModel.EndDate is null)
        {
            await this.ApplyDiscountAsync(requestModel, discount);
        }

        await this.dbContext.SaveChangesAsync();
    }


    public async Task RemoveDiscountAsync(Guid discountId)
    {
        Discount? discount = await this.dbContext.Discounts.FirstOrDefaultAsync(x => x.Id == discountId);
        if (discount is null)
            throw new NotFoundDiscountException("Not found discount with present id");

        if (discount.StartBackgroundJodId is not null && discount.EndBackGroundJodId is not null)
        {
            if (discount.IsActive)
            {
                this.backgroundJob.Delete(discount.EndBackGroundJodId);
                await this.RemoveProductDiscountAsync(discount);
            }

            if (!discount.IsActive)
            {
                this.backgroundJob.Delete(discount.StartBackgroundJodId);
                this.backgroundJob.Delete(discount.EndBackGroundJodId);
                this.dbContext.Discounts.Remove(discount);

                await this.dbContext.SaveChangesAsync();
            }
        }

        if (discount.StartDate is null && discount.EndDate is null)
        {
            await this.RemoveProductDiscountAsync(discount);
            this.dbContext.Discounts.Remove(discount);
        }
    }

    public async Task ApplyDiscountAsync(CreteDiscountRequestModel requestModel, Discount discount)
    {
        if (requestModel.CategoryId is not null)
        {
            bool existsCategory = await this.dbContext.Categories.AnyAsync(x => x.Id == requestModel.CategoryId);
            if (!existsCategory)
            {
                throw new NotFoundCategoryException("Invalid category");
            }

            await this.dbContext.Products
                .Where(x => x.CategoryId == requestModel.CategoryId && x.InStock)
                .ForEachAsync(product =>
                {
                    product.DiscountId = discount.Id;
                    product.Percentage = discount.Percentage;
                    product.NewPrice = product.Price - product.Price * discount.Percentage / 100;
                });
        }

        if (requestModel.SubCategoryId is not null)
        {
            bool existsSubCategory = await this.dbContext.SubCategories.AnyAsync(x => x.Id == requestModel.SubCategoryId);
            if (!existsSubCategory)
            {
                throw new NotFoundCategoryException("Invalid category");
            }


            await this.dbContext.Products
                .Where(x => x.SubCategoryId == requestModel.SubCategoryId && x.InStock)
                .ForEachAsync(product =>
                {
                    product.DiscountId = discount.Id;
                    product.Percentage = discount.Percentage;
                    product.NewPrice = product.Price - product.Price * discount.Percentage / 100;
                });
        }

        discount.IsActive = true;
        await this.dbContext.SaveChangesAsync();
    }

    public async Task RemoveProductDiscountAsync(Discount discount)
    {
        await this.dbContext.Products
            .Where(x => x.DiscountId == discount.Id)
            .ForEachAsync(product =>
            {
                product.DiscountId = null;
                product.Percentage = null;
                product.NewPrice = null;
            });

        this.dbContext.Discounts.Remove(discount);
        await this.dbContext.SaveChangesAsync();
    }
}

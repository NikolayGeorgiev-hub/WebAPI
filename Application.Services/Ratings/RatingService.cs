using Application.Common.Exceptions.Products;
using Application.Common.Exceptions.Ratings;
using Application.Data;
using Application.Data.Models.Products;
using Application.Data.Models.Ratings;
using Application.Data.Models.Users;
using Application.Services.Models.Ratings;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Ratings;

public class RatingService : IRatingService
{
    private readonly ApplicationDbContext dbContext;

    public RatingService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task RateProductAsync(Guid userId, RatingRequestModel requestModel)
    {
        bool existsProduct = await this.dbContext.Products.AnyAsync(x => x.Id == requestModel.ProductId);
        if (!existsProduct)
        {
            throw new NotFoundProductException("Not found product");
        }

        bool isValidValueRange = requestModel.Value > 0 && requestModel.Value < 6;
        if (!isValidValueRange)
        {
            throw new InvalidRatingValueException("Invalid rating value range");
        }


        Rating? userRating = await this.dbContext.Ratings
            .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == requestModel.ProductId);

        if (userRating is null)
        {
            Rating rating = new()
            {
                CreatedOn = DateTime.UtcNow,
                UserId = userId,
                ProductId = requestModel.ProductId,
                Value = requestModel.Value
            };

            await dbContext.AddAsync(rating);
        }
        else
        {
            userRating.Value = requestModel.Value;
            userRating.CreatedOn = DateTime.UtcNow;
        }

        await dbContext.SaveChangesAsync();
    }

    public  RatingResponseModel GetProductRating(Guid productId)
    {
        IReadOnlyList<Rating> ratings =  this.dbContext.Ratings.Where(x => x.ProductId == productId).ToList();

        int ratingsCount = 0;
        double averageRating = 0;

        if (ratings.Count > 0)
        {
            ratingsCount = ratings.Count();
            averageRating = ratings.Select(x => x.Value).Average();
        }

        return new RatingResponseModel(ratingsCount, averageRating);
    }
}

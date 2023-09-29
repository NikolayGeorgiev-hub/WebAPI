using Application.Common.Exceptions.Products;
using Application.Common.Exceptions.Ratings;
using Application.Data.Models.Ratings;
using Application.Data.Repositories.Products;
using Application.Data.Repositories.Ratings;
using Application.Services.Models.Ratings;

namespace Application.Services.Ratings;

public class RatingService : IRatingService
{
    private readonly IRatingRepository ratingRepository;
    private readonly IProductRepository productRepository;

    public RatingService(IRatingRepository ratingRepository, IProductRepository productRepository)
    {
        this.ratingRepository = ratingRepository;
        this.productRepository = productRepository;
    }

    public async Task RateProductAsync(Guid userId, RatingRequestModel requestModel)
    {
        bool existsProduct = await this.productRepository.ExistsProductByIdAsync(userId);
        if (!existsProduct)
            throw new NotFoundProductException("Not found product");


        bool isValidValueRange = requestModel.Value > 0 && requestModel.Value < 6;
        if (!isValidValueRange)
        {
            throw new InvalidRatingValueException("Invalid rating value range");
        }

        Rating? userRating = await this.ratingRepository.GetUserRatingForProductAsync(requestModel.ProductId, userId);
        if (userRating is null)
        {
            Rating rating = new()
            {
                CreatedOn = DateTime.UtcNow,
                UserId = userId,
                ProductId = requestModel.ProductId,
                Value = requestModel.Value
            };

            await this.ratingRepository.AddAsync(rating);
        }
        else
        {
            userRating.Value = requestModel.Value;
            userRating.CreatedOn = DateTime.UtcNow;
        }

        await this.ratingRepository.SaveChangesAsync();
    }
}

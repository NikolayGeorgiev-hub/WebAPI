using Application.Services.Models.Ratings;

namespace Application.Services.Ratings;

public interface IRatingService
{
    Task RateProductAsync(Guid userId, RatingRequestModel requestModel);

   RatingResponseModel GetProductRating(Guid productId);
}

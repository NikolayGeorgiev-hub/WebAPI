using Application.Data.Models.Ratings;

namespace Application.Data.Repositories.Ratings;

public interface IRatingRepository
{
    Task AddAsync(Rating rating);

    Task<Rating?> GetUserRatingForProductAsync(Guid productId, Guid userId);

    Task SaveChangesAsync();
}

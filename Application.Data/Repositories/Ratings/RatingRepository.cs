using Application.Data.Models.Ratings;
using Microsoft.EntityFrameworkCore;

namespace Application.Data.Repositories.Ratings;

public class RatingRepository : IRatingRepository
{
    private readonly ApplicationDbContext dbContext;

    public RatingRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(Rating rating)
    {
        await this.dbContext.Ratings.AddAsync(rating);
    }

    public async Task<Rating?> GetUserRatingForProductAsync(Guid productId, Guid userId)
    {
        Rating? userRating = await this.dbContext.Ratings.FirstOrDefaultAsync(x => x.ProductId == productId && x.UserId == userId);
        return userRating;
    }

    public async Task SaveChangesAsync()
    {
        await this.dbContext.SaveChangesAsync();
    }
}

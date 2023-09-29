namespace Application.Data.Repositories.Categories;

public interface ICategoryRepository
{
    Task ExistsCategoryByIdAsync(Guid categoryId);

    Task ExistsSubCategoryByIdAsync(Guid subCategoryId);

    Task CategoryContainsSubCategoryAsync(Guid subCategoryId, Guid categoryId);
}

using Application.Data.Models.Categories;
using Application.Data.Models.Products;
using static Application.Common.Constants;

namespace Application.Data.Seeding;

public class ProductsSeeder : ISeeder
{
    private readonly List<Category> categories = new List<Category>()
    {
        new Category{ Id = Guid.Parse(ProductSeeder.ElectronicCategoryId), Name = ProductSeeder.ElectronicCategoryName},
        new Category{ Id = Guid.Parse(ProductSeeder.ClothesCategoryId), Name = ProductSeeder.ClothesCategoryName}
    };

    private List<SubCategory> subCategories = new List<SubCategory>()
    {
        new SubCategory{Name = "Laptop-Computers", CategoryId = Guid.Parse(ProductSeeder.ElectronicCategoryId)},
        new SubCategory{Name = "Mobile-Phones", CategoryId = Guid.Parse(ProductSeeder.ElectronicCategoryId)},
        new SubCategory{Name = "Audio-Systems", CategoryId = Guid.Parse(ProductSeeder.ElectronicCategoryId)},
        new SubCategory{Name = "T-shirt", CategoryId = Guid.Parse(ProductSeeder.ClothesCategoryId)},
        new SubCategory{Name = "Jeans", CategoryId = Guid.Parse(ProductSeeder.ClothesCategoryId)},
        new SubCategory{Name = "Jackets", CategoryId = Guid.Parse(ProductSeeder.ClothesCategoryId)},
    };

    public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
    {
        if (dbContext.Categories.Count() == 0)
        {
            await dbContext.Categories.AddRangeAsync(categories);
            await dbContext.SaveChangesAsync();
        }

        if (dbContext.SubCategories.Count() == 0)
        {
            await dbContext.SubCategories.AddRangeAsync(subCategories);
            await dbContext.SaveChangesAsync();
        }

        var user = dbContext.Users.FirstOrDefault();

        if (dbContext.Products.Count() == 0)
        {
            List<Product> products = new()
            {
                new Product
                {
                    Name = "Adidas",
                    Description = "new model t-shirt",
                    Price = 20.50m,
                    Quantity = 150,
                    CategoryId = Guid.Parse(ProductSeeder.ClothesCategoryId),
                    SubCategoryId = dbContext.SubCategories.FirstOrDefault(x => x.Name == "T-shirt")!.Id,
                    OwnerId = user!.Id,
                    InStock = true,
                },
                new Product
                {
                    Name = "Nike",
                    Description = "new model t-shirt",
                    Price = 35.00m,
                    Quantity = 200,
                    CategoryId = Guid.Parse(ProductSeeder.ClothesCategoryId),
                    SubCategoryId = dbContext.SubCategories.FirstOrDefault(x => x.Name == "T-shirt")!.Id,
                    OwnerId = user!.Id,
                    InStock = true,
                },
                new Product
                {
                    Name = "North-Face",
                    Description = "new model jacket",
                    Price = 120,
                    Quantity = 57,
                    CategoryId = Guid.Parse(ProductSeeder.ClothesCategoryId),
                    SubCategoryId = dbContext.SubCategories.FirstOrDefault(x => x.Name == "Jackets")!.Id,
                    OwnerId = user!.Id,
                    InStock = true,
                },
                new Product
                {
                    Name = "North-Face",
                    Description = "new model jacket",
                    Price = 220.30m,
                    Quantity = 72,
                    CategoryId = Guid.Parse(ProductSeeder.ClothesCategoryId),
                    SubCategoryId = dbContext.SubCategories.FirstOrDefault(x => x.Name == "Jackets")!.Id,
                    OwnerId = user!.Id,
                    InStock = true,
                },
                new Product
                {
                    Name = "Lenovo",
                    Description = "new model laptop",
                    Price = 2500,
                    Quantity = 10,
                    CategoryId = Guid.Parse(ProductSeeder.ElectronicCategoryId),
                    SubCategoryId = dbContext.SubCategories.FirstOrDefault(x => x.Name == "Laptop-Computers")!.Id,
                    OwnerId = user!.Id,
                    InStock = true,
                },
                new Product
                {
                    Name = "Lenovo Z590",
                    Description = "new model laptop",
                    Price = 1900,
                    Quantity = 10,
                    CategoryId = Guid.Parse(ProductSeeder.ElectronicCategoryId),
                    SubCategoryId = dbContext.SubCategories.FirstOrDefault(x => x.Name == "Laptop-Computers")!.Id,
                    OwnerId = user!.Id,
                    InStock = true,
                },
                new Product
                {
                    Name = "IPhone 14",
                    Description = "new model phone",
                    Price = 2300.20m,
                    Quantity = 15,
                    CategoryId = Guid.Parse(ProductSeeder.ElectronicCategoryId),
                    SubCategoryId = dbContext.SubCategories.FirstOrDefault(x => x.Name == "Mobile-Phones")!.Id,
                    OwnerId = user!.Id,
                    InStock = true,
                },
                new Product
                {
                    Name = "Samsung",
                    Description = "new model phone",
                    Price = 1300.20m,
                    Quantity = 15,
                    CategoryId = Guid.Parse(ProductSeeder.ElectronicCategoryId),
                    SubCategoryId = dbContext.SubCategories.FirstOrDefault(x => x.Name == "Mobile-Phones")!.Id,
                    OwnerId = user!.Id,
                    InStock = true,
                }
            };

            await dbContext.AddRangeAsync(products);
            await dbContext.SaveChangesAsync();
        }

    }
}

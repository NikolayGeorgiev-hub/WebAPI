﻿using Application.Data.Models.Categories;
using Application.Data.Models.Comments;
using Application.Data.Models.Discounts;
using Application.Data.Models.Orders;
using Application.Data.Models.Products;
using Application.Data.Models.Ratings;
using Application.Data.Models.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        this.ConfigureApplicationRelations(builder);
    }

    public DbSet<Category> Categories { get; set; }

    public DbSet<SubCategory> SubCategories { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<Rating> Ratings { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<ProductsList> ProductsLists { get; set; }

    public DbSet<Comment> Comments { get; set; }

    public DbSet<Discount> Discounts { get; set; }

    public DbSet<OrderDetails> OrderDetails { get; set; }

    public DbSet<OrderProductDetails> OrderProducts { get; set; }
    private void ConfigureApplicationRelations(ModelBuilder builder)
         => builder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

}

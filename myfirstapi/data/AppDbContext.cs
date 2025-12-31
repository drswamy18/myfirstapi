using Microsoft.EntityFrameworkCore;
using ProductApi.Models;

namespace ProductApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options) 
        {
        }
        
        public DbSet<Product> Products { get; set; }
        
        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     // Seed initial data
        //     modelBuilder.Entity<Product>().HasData(
        //         new Product 
        //         { 
        //             Id = 1, 
        //             Name = "Laptop", 
        //             Description = "High-performance laptop", 
        //             Price = 999.99m, 
        //             Stock = 10,
        //             CreatedAt = DateTime.UtcNow
        //         },
        //         new Product 
        //         { 
        //             Id = 2, 
        //             Name = "Smartphone", 
        //             Description = "Latest smartphone", 
        //             Price = 699.99m, 
        //             Stock = 25,
        //             CreatedAt = DateTime.UtcNow
        //         },
        //         new Product
        //         { 
        //             Id = 3, 
        //             Name = "iPhone", 
        //             Description = "iPhone 17 Pro max is realsed Now", 
        //             Price = 699.99m, 
        //             Stock = 25,
        //             CreatedAt = DateTime.UtcNow
        //         }
        //     );
        // }
    }
}
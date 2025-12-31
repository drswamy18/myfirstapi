using LoginApi.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LoginApi.Data
{
    public class LoginDbContext : DbContext
    {
        public LoginDbContext(DbContextOptions<LoginDbContext> options):base(options)
        {
            
        }
        public DbSet<Login> login {get;set;}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        modelBuilder.Entity<Login>().HasData(
            new Login
            {
                Id= 1,
                Name="Test",
                password="Password@123"
            }
        );
        }
    }
}
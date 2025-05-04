using BackendDev_Case1_Rozul.Entities;
using Microsoft.EntityFrameworkCore;
namespace BackendDev_Case1_Rozul.Database { 
    public class ApplicationDbContext : DbContext{
        // Config/secret.json
        public IConfiguration config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("Config/secret.json", optional: false, reloadOnChange: true).Build();
        public static string? ConnectionString {get; private set;}
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<CategoryType> CategoryTypes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Retrieve Connection String from Secret.json
            ConnectionString = config["ConnectionString"] ?? throw new Exception("Connection String is Invalid");
            // Connect to SQL Server
            optionsBuilder.UseSqlServer(ConnectionString);
        }

    }
}
using Microsoft.EntityFrameworkCore;
using Proiect.Models;


namespace Proiect.Data;

public class NutBuddiesContext : DbContext
{
    public NutBuddiesContext(DbContextOptions<NutBuddiesContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Review> Reviews => Set<Review>();
    
    public DbSet<Customer> Customers => Set<Customer>();


}
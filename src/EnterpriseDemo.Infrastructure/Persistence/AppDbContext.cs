using EnterpriseDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseDemo.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var customer = modelBuilder.Entity<Customer>();
        customer.ToTable("Customers");
        customer.HasKey(x => x.Id);
        customer.Property(x => x.FirstName).HasMaxLength(80).IsRequired();
        customer.Property(x => x.LastName).HasMaxLength(80).IsRequired();
        customer.Property(x => x.Email).HasMaxLength(254).IsRequired();
        customer.HasIndex(x => x.Email).IsUnique();
        customer.Property(x => x.CreatedUtc).IsRequired();
    }
}

using Microsoft.EntityFrameworkCore;
using InvoiceGenerator.API.Models;

namespace InvoiceGenerator.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Invoice>()
            .Property(i => i.Tax).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Invoice>()
            .Property(i => i.Discount).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<InvoiceItem>()
            .Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
    }
}

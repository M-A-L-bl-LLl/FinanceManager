using FinanceManager.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Data;

public class AppDbContext : DbContext
{
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Budget> Budgets => Set<Budget>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>(e =>
        {
            e.Property(t => t.Amount).HasColumnType("decimal(18,2)");
            e.HasOne(t => t.Category)
             .WithMany(c => c.Transactions)
             .HasForeignKey(t => t.CategoryId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Budget>(e =>
        {
            e.Property(b => b.Limit).HasColumnType("decimal(18,2)");
            e.HasOne(b => b.Category)
             .WithMany(c => c.Budgets)
             .HasForeignKey(b => b.CategoryId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(b => new { b.CategoryId, b.Month, b.Year }).IsUnique();
        });

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Зарплата", Icon = "CurrencyUsd", Color = "#4CAF50", Type = TransactionType.Income },
            new Category { Id = 2, Name = "Фриланс", Icon = "Laptop", Color = "#2196F3", Type = TransactionType.Income },
            new Category { Id = 3, Name = "Еда", Icon = "FoodForkDrink", Color = "#FF9800", Type = TransactionType.Expense },
            new Category { Id = 4, Name = "Транспорт", Icon = "Car", Color = "#9C27B0", Type = TransactionType.Expense },
            new Category { Id = 5, Name = "Развлечения", Icon = "GamepadVariant", Color = "#E91E63", Type = TransactionType.Expense },
            new Category { Id = 6, Name = "Здоровье", Icon = "Hospital", Color = "#F44336", Type = TransactionType.Expense },
            new Category { Id = 7, Name = "Прочее", Icon = "DotsHorizontal", Color = "#607D8B", Type = TransactionType.Expense }
        );
    }
}

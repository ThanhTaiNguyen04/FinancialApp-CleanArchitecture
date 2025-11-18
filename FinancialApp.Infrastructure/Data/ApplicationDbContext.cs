using Microsoft.EntityFrameworkCore;
using FinancialApp.Domain.Entities;

namespace FinancialApp.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Transfer> Transfers { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Budget> Budgets { get; set; }
    public DbSet<SavingGoal> SavingGoals { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<PremiumRequest> PremiumRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Remove all SQL Server-specific column type annotations
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entity.GetProperties())
            {
                var annotations = property.GetAnnotations()
                    .Where(a => a.Name == "Relational:ColumnType")
                    .ToList();
                
                foreach (var annotation in annotations)
                {
                    property.RemoveAnnotation(annotation.Name);
                }
            }
        }

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.AvailableBalance).HasDefaultValue(0);
        });

        // Configure Transaction entity
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasOne(d => d.User)
                .WithMany(p => p.Transactions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.UserId, e.TransactionDate });
        });

        // Configure Contact entity
        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasOne(d => d.User)
                .WithMany(p => p.Contacts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.UserId, e.Phone }).IsUnique();
        });

        // Configure Transfer entity
        modelBuilder.Entity<Transfer>(entity =>
        {
            entity.HasOne(d => d.FromUser)
                .WithMany(p => p.SentTransfers)
                .HasForeignKey(d => d.FromUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.ToContact)
                .WithMany(p => p.ReceivedTransfers)
                .HasForeignKey(d => d.ToContactId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.FromUserId, e.TransferDate });
        });

        // Configure PremiumRequest entity
        modelBuilder.Entity<PremiumRequest>(entity =>
        {
            entity.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.ApprovedByUser)
                .WithMany()
                .HasForeignKey(d => d.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.UserId, e.RequestDate });
            entity.HasIndex(e => e.Status);
        });

        // Configure ChatMessage entity
        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.UserId);
        });

        // Seed data - Temporarily disabled for PostgreSQL migration
        // SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Use static dates instead of DateTime.UtcNow for seed data
        var seedDate = new DateTime(2025, 9, 26, 10, 0, 0, DateTimeKind.Utc);
        
        // Seed Users
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                FullName = "Christopher Summers",
                Email = "christopher.summers@email.com",
                Phone = "+1234567890",
                AvatarUrl = "https://images.pexels.com/photos/2167673/pexels-photo-2167673.jpeg?auto=compress&cs=tinysrgb&dpr=3&h=750&w=1260",
                AvailableBalance = 5320.50m,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            }
        );

        // Seed Contacts
        modelBuilder.Entity<Contact>().HasData(
            new Contact { Id = 1, UserId = 1, FullName = "Catherine Johnson", Phone = "+1234567891", Email = "catherine@email.com", AvatarUrl = "https://images.pexels.com/photos/733872/pexels-photo-733872.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=750&w=1260", IsRecent = true, CreatedAt = seedDate },
            new Contact { Id = 2, UserId = 1, FullName = "Allan Smith", Phone = "+1234567892", Email = "allan@email.com", AvatarUrl = "https://images.pexels.com/photos/697509/pexels-photo-697509.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=750&w=1260", IsRecent = true, CreatedAt = seedDate },
            new Contact { Id = 3, UserId = 1, FullName = "Kimberly Brown", Phone = "+1234567893", Email = "kimberly@email.com", AvatarUrl = "https://images.pexels.com/photos/2748091/pexels-photo-2748091.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=750&w=1260", IsRecent = true, CreatedAt = seedDate }
        );

        // Seed Transactions
        modelBuilder.Entity<Transaction>().HasData(
            new Transaction { Id = 1, UserId = 1, Type = "income", Category = "Lương", Amount = 16000000m, Description = "Lương tháng 9", TransactionDate = seedDate.AddDays(-5), CreatedAt = seedDate },
            new Transaction { Id = 2, UserId = 1, Type = "expense", Category = "Đi chợ", Amount = 455789m, Description = "Mua sắm hàng tuần", TransactionDate = seedDate.AddDays(-3), CreatedAt = seedDate },
            new Transaction { Id = 3, UserId = 1, Type = "expense", Category = "Thuê nhà", Amount = 3000000m, Description = "Tiền thuê nhà tháng 9", TransactionDate = seedDate.AddDays(-2), CreatedAt = seedDate },
            new Transaction { Id = 4, UserId = 1, Type = "expense", Category = "Y tế", Amount = 243789m, Description = "Khám bệnh và thuốc", TransactionDate = seedDate.AddDays(-1), CreatedAt = seedDate }
        );

        // Seed Categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Lương", IconName = "salary", ColorCode = "#4CAF50", Type = "income", IsActive = true, CreatedAt = seedDate },
            new Category { Id = 2, Name = "Đi chợ", IconName = "shopping", ColorCode = "#FF9800", Type = "expense", IsActive = true, CreatedAt = seedDate },
            new Category { Id = 3, Name = "Thuê nhà", IconName = "home", ColorCode = "#2196F3", Type = "expense", IsActive = true, CreatedAt = seedDate },
            new Category { Id = 4, Name = "Y tế", IconName = "medical", ColorCode = "#F44336", Type = "expense", IsActive = true, CreatedAt = seedDate },
            new Category { Id = 5, Name = "Du lịch", IconName = "travel", ColorCode = "#9C27B0", Type = "expense", IsActive = true, CreatedAt = seedDate }
        );

        // Seed Budgets
        modelBuilder.Entity<Budget>().HasData(
            new Budget { Id = 1, UserId = 1, CategoryId = 2, BudgetAmount = 2000000m, SpentAmount = 455789m, Month = 9, Year = 2025, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Budget { Id = 2, UserId = 1, CategoryId = 3, BudgetAmount = 3500000m, SpentAmount = 3000000m, Month = 9, Year = 2025, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Budget { Id = 3, UserId = 1, CategoryId = 4, BudgetAmount = 1000000m, SpentAmount = 243789m, Month = 9, Year = 2025, CreatedAt = seedDate, UpdatedAt = seedDate }
        );

        // Seed Saving Goals
        modelBuilder.Entity<SavingGoal>().HasData(
            new SavingGoal { Id = 1, UserId = 1, Name = "Giáo dục", Description = "Tiết kiệm cho việc học", TargetAmount = 5000000m, CurrentAmount = 2300000m, TargetDate = seedDate.AddMonths(6), IconName = "education", ColorCode = "#4CAF50", Status = "active", CreatedAt = seedDate, UpdatedAt = seedDate },
            new SavingGoal { Id = 2, UserId = 1, Name = "Mua nhà", Description = "Tiết kiệm mua nhà", TargetAmount = 47000000m, CurrentAmount = 5600000m, TargetDate = seedDate.AddYears(2), IconName = "house", ColorCode = "#FF9800", Status = "active", CreatedAt = seedDate, UpdatedAt = seedDate },
            new SavingGoal { Id = 3, UserId = 1, Name = "Du lịch", Description = "Chuyến du lịch Hàn Quốc", TargetAmount = 2200000m, CurrentAmount = 800000m, TargetDate = seedDate.AddMonths(4), IconName = "travel", ColorCode = "#2196F3", Status = "active", CreatedAt = seedDate, UpdatedAt = seedDate }
        );
    }
}
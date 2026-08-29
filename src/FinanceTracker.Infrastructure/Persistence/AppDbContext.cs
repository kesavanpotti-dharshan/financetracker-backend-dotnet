using FinanceTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Institution> Institutions => Set<Institution>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<AccountBalance> AccountBalances => Set<AccountBalance>();
    public DbSet<CreditCardDetails> CreditCardDetails => Set<CreditCardDetails>();
    public DbSet<Statement> Statements => Set<Statement>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CreditCardDetails>()
            .HasKey(c => c.AccountId); // 1:1, AccountId is both PK and FK

        modelBuilder.Entity<Account>()
            .HasOne(a => a.CreditCardDetails)
            .WithOne(c => c.Account)
            .HasForeignKey<CreditCardDetails>(c => c.AccountId);

        modelBuilder.Entity<Statement>()
            .Property(s => s.RawExtractedJson)
            .HasColumnType("jsonb");

        modelBuilder.Entity<Account>()
            .Property(a => a.AccountType)
            .HasConversion<string>(); // store enums as text, not int — readable in DB, safe if you reorder enum values later

        modelBuilder.Entity<AccountBalance>()
            .Property(b => b.Source)
            .HasConversion<string>();

        modelBuilder.Entity<Statement>()
            .Property(s => s.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Account>()
            .HasIndex(a => a.UserId);

        modelBuilder.Entity<AccountBalance>()
            .HasIndex(b => new { b.AccountId, b.AsOfDate });

        modelBuilder.Entity<RefreshToken>()
            .HasIndex(r => r.UserId);

        modelBuilder.Entity<RefreshToken>()
            .HasIndex(r => r.TokenHash)
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}
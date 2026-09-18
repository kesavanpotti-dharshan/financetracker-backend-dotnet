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
        // Credit card details — 1:1 with Account, AccountId is both PK and FK
        modelBuilder.Entity<CreditCardDetails>()
            .HasKey(c => c.AccountId);

        modelBuilder.Entity<Account>()
            .HasOne(a => a.CreditCardDetails)
            .WithOne(c => c.Account)
            .HasForeignKey<CreditCardDetails>(c => c.AccountId);

        // Statements — raw AI extraction stored as jsonb for flexibility/audit trail
        modelBuilder.Entity<Statement>()
            .Property(s => s.RawExtractedJson)
            .HasColumnType("jsonb");

        // Store enums as text — readable in the DB, safe against reordering enum values later
        modelBuilder.Entity<Account>()
            .Property(a => a.AccountType)
            .HasConversion<string>();

        modelBuilder.Entity<AccountBalance>()
            .Property(b => b.Source)
            .HasConversion<string>();

        modelBuilder.Entity<Statement>()
            .Property(s => s.Status)
            .HasConversion<string>();

        // Indexes for the queries that run constantly
        modelBuilder.Entity<Account>()
            .HasIndex(a => a.UserId);

        modelBuilder.Entity<AccountBalance>()
            .HasIndex(b => new { b.AccountId, b.AsOfDate });

        modelBuilder.Entity<RefreshToken>()
            .HasIndex(r => r.UserId);

        modelBuilder.Entity<RefreshToken>()
            .HasIndex(r => r.TokenHash)
            .IsUnique();

        // Seed data — common institutions available to every user out of the box
        modelBuilder.Entity<Institution>().HasData(
            // Banks
            new Institution { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Name = "Chase", Type = "Bank" },
            new Institution { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Name = "BMO", Type = "Bank" },
            new Institution { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Name = "Wells Fargo", Type = "Bank" },
            new Institution { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), Name = "Citibank", Type = "Bank" },
            new Institution { Id = Guid.Parse("10000000-0000-0000-0000-000000000005"), Name = "US Bank", Type = "Bank" },
            new Institution { Id = Guid.Parse("10000000-0000-0000-0000-000000000006"), Name = "PNC Bank", Type = "Bank" },
            new Institution { Id = Guid.Parse("10000000-0000-0000-0000-000000000007"), Name = "Associated Bank", Type = "Bank" },
            new Institution { Id = Guid.Parse("10000000-0000-0000-0000-000000000008"), Name = "Capital One", Type = "Bank" },
            new Institution { Id = Guid.Parse("10000000-0000-0000-0000-000000000009"), Name = "Ally Bank", Type = "Bank" },

            // Credit unions
            new Institution { Id = Guid.Parse("10000000-0000-0000-0000-00000000000a"), Name = "Navy Federal Credit Union", Type = "Credit Union" },

            // Online-first banks
            new Institution { Id = Guid.Parse("10000000-0000-0000-0000-00000000000b"), Name = "Chime", Type = "Bank" },
            new Institution { Id = Guid.Parse("10000000-0000-0000-0000-00000000000c"), Name = "SoFi", Type = "Bank" },
            new Institution { Id = Guid.Parse("10000000-0000-0000-0000-00000000000d"), Name = "Marcus by Goldman Sachs", Type = "Bank" },

            // Brokerages / investment
            new Institution { Id = Guid.Parse("10000000-0000-0000-0000-00000000000e"), Name = "Fidelity", Type = "Brokerage" },
            new Institution { Id = Guid.Parse("10000000-0000-0000-0000-00000000000f"), Name = "Vanguard", Type = "Brokerage" },
            new Institution { Id = Guid.Parse("10000000-0000-0000-0000-000000000010"), Name = "Charles Schwab", Type = "Brokerage" },
            new Institution { Id = Guid.Parse("10000000-0000-0000-0000-000000000011"), Name = "E*TRADE", Type = "Brokerage" },
            new Institution { Id = Guid.Parse("10000000-0000-0000-0000-000000000012"), Name = "Robinhood", Type = "Brokerage" },

            // Credit card issuers
            new Institution { Id = Guid.Parse("10000000-0000-0000-0000-000000000013"), Name = "American Express", Type = "Card Issuer" },
            new Institution { Id = Guid.Parse("10000000-0000-0000-0000-000000000014"), Name = "Discover", Type = "Card Issuer" }
        );

        base.OnModelCreating(modelBuilder);
    }
}
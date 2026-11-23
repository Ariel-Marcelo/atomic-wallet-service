using AtomicWallet.Api.Common.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace AtomicWallet.Api.Common.Persistence;

public class WalletDbContext(DbContextOptions<WalletDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();
    public DbSet<Transfer> Transfers => Set<Transfer>();
    public DbSet<LedgerEntry> LedgerEntries => Set<LedgerEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. Configuración de Account (Control Optimista + Precisión Bancaria)
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.HasIndex(a => a.AccountNumber).IsUnique();
            entity.Property(a => a.AccountNumber).HasMaxLength(30).IsRequired();
            entity.Property(a => a.OwnerName).HasMaxLength(150).IsRequired();
            entity.Property(a => a.Currency).HasMaxLength(3).IsRequired();
            entity.Property(a => a.Balance).HasPrecision(18, 4);

            // Aquí le decimos a EF Core que esta columna es un rowversion de SQL Server
            // y debe incluirse en la cláusula WHERE de cada UPDATE
            entity.Property(a => a.RowVersion).IsRowVersion();
        });

        // 2. Configuración de IdempotencyRecord (Lock Atómico por PK)
        modelBuilder.Entity<IdempotencyRecord>(entity =>
        {
            entity.HasKey(i => i.IdempotencyKey);
            entity.Property(i => i.IdempotencyKey).HasMaxLength(100);
            entity.Property(i => i.RequestHash).HasMaxLength(64).IsRequired();
            entity.Property(i => i.Status).HasConversion<string>().HasMaxLength(20);
        });

        // 3. Configuración de Transfer
        modelBuilder.Entity<Transfer>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.HasIndex(t => t.IdempotencyKey).IsUnique();
            entity.Property(t => t.IdempotencyKey).HasMaxLength(100).IsRequired();
            entity.Property(t => t.Amount).HasPrecision(18, 4);
            entity.Property(t => t.Currency).HasMaxLength(3).IsRequired();
            entity.Property(t => t.Reference).HasMaxLength(250);
        });

        // 4. Configuración de LedgerEntry (Inmutable)
        modelBuilder.Entity<LedgerEntry>(entity =>
        {
            entity.HasKey(l => l.Id);
            entity.HasIndex(l => l.TransferId);
            entity.HasIndex(l => l.AccountId);
            entity.Property(l => l.EntryType).HasConversion<string>().HasMaxLength(10);
            entity.Property(l => l.Amount).HasPrecision(18, 4);
            entity.Property(l => l.BalanceAfter).HasPrecision(18, 4);
        });
    }
}
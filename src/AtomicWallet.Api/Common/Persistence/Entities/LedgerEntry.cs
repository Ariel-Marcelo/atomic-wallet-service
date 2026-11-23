namespace AtomicWallet.Api.Common.Persistence.Entities;

public enum LedgerEntryType
{
    Debit = 1,
    Credit = 2
}

public class LedgerEntry
{
    public Guid Id { get; private set; }
    public Guid TransferId { get; private set; }
    public Guid AccountId { get; private set; }
    public LedgerEntryType EntryType { get; private set; }
    public decimal Amount { get; private set; }
    
    // Snapshot auditable del saldo exacto que quedó en la cuenta tras este movimiento
    public decimal BalanceAfter { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private LedgerEntry() { }

    public LedgerEntry(Guid transferId, Guid accountId, LedgerEntryType entryType, decimal amount, decimal balanceAfter)
    {
        Id = Guid.NewGuid();
        TransferId = transferId;
        AccountId = accountId;
        EntryType = entryType;
        Amount = amount;
        BalanceAfter = balanceAfter;
        CreatedAt = DateTime.UtcNow;
    }
}
namespace AtomicWallet.Api.Common.Persistence.Entities;

public class Transfer
{
    public Guid Id { get; private set; }
    public string IdempotencyKey { get; private set; } = string.Empty;
    public Guid SourceAccountId { get; private set; }
    public Guid DestinationAccountId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public string Reference { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private Transfer() { }

    public Transfer(string idempotencyKey, Guid sourceAccountId, Guid destinationAccountId, decimal amount, string currency, string reference)
    {
        Id = Guid.NewGuid();
        IdempotencyKey = idempotencyKey;
        SourceAccountId = sourceAccountId;
        DestinationAccountId = destinationAccountId;
        Amount = amount;
        Currency = currency.ToUpperInvariant();
        Reference = reference;
        CreatedAt = DateTime.UtcNow;
    }
}

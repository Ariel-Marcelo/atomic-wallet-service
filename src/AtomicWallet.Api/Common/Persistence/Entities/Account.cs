namespace AtomicWallet.Api.Common.Persistence.Entities;

public class Account
{
    public Guid Id { get; private set; }
    public string AccountNumber { get; private set; } = string.Empty;
    public string OwnerName { get; private set; } = string.Empty;
    public decimal Balance { get; private set; }
    public string Currency { get; private set; } = string.Empty;

    // Token de Concurrencia Optimista nativo de SQL Server (rowversion).
    public byte[] RowVersion { get; private set; } = [];

    public DateTime UpdatedAt { get; private set; }

    private Account() { }

    public Account(string accountNumber, string ownerName, decimal initialBalance, string currency)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            throw new ArgumentException("El número de cuenta es obligatorio.", nameof(accountNumber));

        if (string.IsNullOrWhiteSpace(ownerName))
            throw new ArgumentException("El titular de la cuenta es obligatorio.", nameof(ownerName));

        if (initialBalance < 0)
            throw new ArgumentException("El saldo inicial no puede ser negativo.", nameof(initialBalance));

        if (string.IsNullOrWhiteSpace(currency) || currency.Trim().Length != 3)
            throw new ArgumentException("La moneda debe ser un código ISO 4217 válido de 3 letras (ej. USD, PEN, EUR).", nameof(currency));

        Id = Guid.NewGuid();
        AccountNumber = accountNumber.Trim();
        OwnerName = ownerName.Trim();
        Balance = initialBalance;
        Currency = currency.Trim().ToUpperInvariant();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Debit(decimal amount)
    {
        if (amount <= 0)
            throw new InvalidOperationException("El monto a debitar debe ser mayor a cero.");

        if (Balance < amount)
            throw new InvalidOperationException($"Fondos insuficientes en la cuenta {AccountNumber}. Saldo actual: {Balance:F2}, Solicitado: {amount:F2}");

        Balance -= amount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Credit(decimal amount)
    {
        if (amount <= 0)
            throw new InvalidOperationException("El monto a acreditar debe ser mayor a cero.");

        Balance += amount;
        UpdatedAt = DateTime.UtcNow;
    }
}

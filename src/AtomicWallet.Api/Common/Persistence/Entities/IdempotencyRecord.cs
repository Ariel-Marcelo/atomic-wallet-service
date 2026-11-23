namespace AtomicWallet.Api.Common.Persistence.Entities;

public enum IdempotencyStatus
{
    InProgress = 1,
    Completed = 2
}

public class IdempotencyRecord
{
    public string IdempotencyKey { get; private set; } = string.Empty;
    
    // SHA-256 del request body para detectar si el cliente reutiliza la misma llave con otro monto/cuenta
    public string RequestHash { get; private set; } = string.Empty;
    
    public IdempotencyStatus Status { get; private set; }
    public int? StatusCode { get; private set; }
    public string? ResponsePayload { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    // Protección contra "Zombie Locks": si el pod/servidor muere a mitad de vuelo,
    // después de este timestamp otro reintento puede reclamar el lock expirado.
    public DateTime LockedUntil { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private IdempotencyRecord() { }

    public IdempotencyRecord(string idempotencyKey, string requestHash, TimeSpan lockDuration)
    {
        IdempotencyKey = idempotencyKey;
        RequestHash = requestHash;
        Status = IdempotencyStatus.InProgress;
        CreatedAt = DateTime.UtcNow;
        LockedUntil = CreatedAt.Add(lockDuration);
    }

    public void MarkAsCompleted(int statusCode, string responsePayload)
    {
        Status = IdempotencyStatus.Completed;
        StatusCode = statusCode;
        ResponsePayload = responsePayload;
        CompletedAt = DateTime.UtcNow;
    }

    public bool IsLockExpired() =>
        Status == IdempotencyStatus.InProgress && DateTime.UtcNow > LockedUntil;

    public void RenewLock(TimeSpan lockDuration)
    {
        LockedUntil = DateTime.UtcNow.Add(lockDuration);
    }
}
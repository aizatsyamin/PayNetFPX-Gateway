namespace PayNetFPX.Gateway.Data.Entities;

public class Transaction
{
    public string TransactionId { get; set; } = Guid.NewGuid().ToString();
    public string PaymentId { get; set; } = string.Empty;
    public string BankCode { get; set; } = string.Empty;
    public string? BankName { get; set; }
    public string? ReferenceNumber { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "MYR";
    public string? ResponseCode { get; set; }
    public string? ResponseMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public string? CustomerReference { get; set; }

    public virtual Payment? Payment { get; set; }
}

public enum TransactionStatus
{
    Pending = 0,
    Processing = 1,
    Success = 2,
    Failed = 3,
    Cancelled = 4
}

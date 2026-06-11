namespace PayNetFPX.Gateway.Data.Entities;

public class Payment
{
    public string PaymentId { get; set; } = Guid.NewGuid().ToString();
    public string MerchantId { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "MYR";
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string Description { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public string? ReturnUrl { get; set; }
    public string? CallbackUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? Metadata { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}

public enum PaymentStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3,
    Cancelled = 4,
    Expired = 5,
    Refunded = 6
}

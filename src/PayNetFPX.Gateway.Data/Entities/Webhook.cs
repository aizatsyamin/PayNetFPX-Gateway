namespace PayNetFPX.Gateway.Data.Entities;

public class Webhook
{
    public string WebhookId { get; set; } = Guid.NewGuid().ToString();
    public string MerchantId { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Secret { get; set; }
    public string Events { get; set; } = string.Empty; // Comma-separated list of events
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastTriggeredAt { get; set; }
    public int FailureCount { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    public virtual Merchant? Merchant { get; set; }
}

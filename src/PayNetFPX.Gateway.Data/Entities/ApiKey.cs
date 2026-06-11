namespace PayNetFPX.Gateway.Data.Entities;

public class ApiKey
{
    public string ApiKeyId { get; set; } = Guid.NewGuid().ToString();
    public string MerchantId { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string Environment { get; set; } = "Development";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastUsedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual Merchant? Merchant { get; set; }
}

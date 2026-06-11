namespace PayNetFPX.Gateway.Data.Entities;

public class Merchant
{
    public string MerchantId { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? BusinessType { get; set; }
    public string? BusinessRegistration { get; set; }
    public MerchantStatus Status { get; set; } = MerchantStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public virtual ICollection<ApiKey> ApiKeys { get; set; } = new List<ApiKey>();
    public virtual ICollection<Webhook> Webhooks { get; set; } = new List<Webhook>();
}

public enum MerchantStatus
{
    Pending = 0,
    Active = 1,
    Suspended = 2,
    Inactive = 3
}

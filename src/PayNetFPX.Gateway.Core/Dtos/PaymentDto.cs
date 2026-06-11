namespace PayNetFPX.Gateway.Core.Dtos;

public class PaymentInitiateRequest
{
    public string MerchantId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "MYR";
    public string Reference { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public string ReturnUrl { get; set; } = string.Empty;
    public string CallbackUrl { get; set; } = string.Empty;
    public int? ExpiresIn { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}

public class PaymentResponse
{
    public bool Success { get; set; }
    public PaymentResponseData? Data { get; set; }
    public ErrorResponse? Error { get; set; }
}

public class PaymentResponseData
{
    public string PaymentId { get; set; } = string.Empty;
    public string RedirectUrl { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class ErrorResponse
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, string>? Details { get; set; }
    public string RequestId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class PaymentDetailsResponse
{
    public string PaymentId { get; set; } = string.Empty;
    public string MerchantId { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public List<TransactionDto> Transactions { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}

public class TransactionDto
{
    public string TransactionId { get; set; } = string.Empty;
    public string BankCode { get; set; } = string.Empty;
    public string? BankName { get; set; }
    public string? ReferenceNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class VerifyPaymentRequest
{
    public bool ForceRefresh { get; set; }
}

public class VerifyPaymentResponse
{
    public string PaymentId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool Verified { get; set; }
    public DateTime LastVerifiedAt { get; set; }
    public string PaynetStatus { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

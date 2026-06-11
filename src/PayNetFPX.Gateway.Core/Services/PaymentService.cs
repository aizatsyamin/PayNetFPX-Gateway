namespace PayNetFPX.Gateway.Core.Services;

using PayNetFPX.Gateway.Data.Entities;
using PayNetFPX.Gateway.Data.Repositories;

public interface IPaymentService
{
    Task<Payment> InitiatePaymentAsync(string merchantId, decimal amount, string currency, 
        string reference, string description, string? customerEmail, string? customerPhone, 
        string returnUrl, string callbackUrl, int? expiresInMinutes = null, 
        Dictionary<string, string>? metadata = null);
    
    Task<Payment?> GetPaymentAsync(string paymentId);
    Task<Payment?> VerifyPaymentAsync(string paymentId, bool forceRefresh = false);
    Task<bool> CancelPaymentAsync(string paymentId, string reason = "");
    Task UpdatePaymentStatusAsync(string paymentId, PaymentStatus status);
    Task<IEnumerable<Payment>> GetMerchantPaymentsAsync(string merchantId, int page = 1, int pageSize = 20);
}

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IPayNetClient _payNetClient;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(
        IPaymentRepository paymentRepository,
        ITransactionRepository transactionRepository,
        IPayNetClient payNetClient,
        ILogger<PaymentService> logger)
    {
        _paymentRepository = paymentRepository;
        _transactionRepository = transactionRepository;
        _payNetClient = payNetClient;
        _logger = logger;
    }

    public async Task<Payment> InitiatePaymentAsync(
        string merchantId, 
        decimal amount, 
        string currency,
        string reference, 
        string description, 
        string? customerEmail, 
        string? customerPhone,
        string returnUrl, 
        string callbackUrl, 
        int? expiresInMinutes = null,
        Dictionary<string, string>? metadata = null)
    {
        // Validate input
        if (amount <= 0 || amount > 999999.99m)
            throw new ArgumentException("Amount must be between 0.01 and 999999.99");

        if (string.IsNullOrWhiteSpace(reference))
            throw new ArgumentException("Reference is required");

        // Check for duplicate reference
        var existingPayment = await _paymentRepository.GetByReferenceAsync(merchantId, reference);
        if (existingPayment != null)
            throw new InvalidOperationException("A payment with this reference already exists");

        // Create payment entity
        var payment = new Payment
        {
            PaymentId = Guid.NewGuid().ToString(),
            MerchantId = merchantId,
            Reference = reference,
            Amount = amount,
            Currency = currency,
            Description = description,
            CustomerEmail = customerEmail,
            CustomerPhone = customerPhone,
            ReturnUrl = returnUrl,
            CallbackUrl = callbackUrl,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiresInMinutes ?? 1440),
            Metadata = metadata != null ? System.Text.Json.JsonSerializer.Serialize(metadata) : null
        };

        // Save to database
        await _paymentRepository.AddAsync(payment);

        _logger.LogInformation(
            "Payment initiated: {PaymentId}, Amount: {Amount}, MerchantId: {MerchantId}",
            payment.PaymentId, amount, merchantId);

        return payment;
    }

    public async Task<Payment?> GetPaymentAsync(string paymentId)
    {
        return await _paymentRepository.GetByIdAsync(paymentId);
    }

    public async Task<Payment?> VerifyPaymentAsync(string paymentId, bool forceRefresh = false)
    {
        var payment = await _paymentRepository.GetByIdAsync(paymentId);
        if (payment == null)
            return null;

        // If already completed, return as is
        if (payment.Status == PaymentStatus.Completed && !forceRefresh)
            return payment;

        try
        {
            // Call PayNet to verify status
            var paynetResult = await _payNetClient.VerifyPaymentAsync(paymentId);
            
            if (paynetResult.IsSuccessful)
            {
                payment.Status = PaymentStatus.Completed;
                payment.CompletedAt = DateTime.UtcNow;
                payment.UpdatedAt = DateTime.UtcNow;
                await _paymentRepository.UpdateAsync(payment);

                _logger.LogInformation("Payment verified successfully: {PaymentId}", paymentId);
            }
            else
            {
                payment.Status = PaymentStatus.Failed;
                payment.UpdatedAt = DateTime.UtcNow;
                await _paymentRepository.UpdateAsync(payment);

                _logger.LogWarning("Payment verification failed: {PaymentId}, Reason: {Reason}", 
                    paymentId, paynetResult.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying payment: {PaymentId}", paymentId);
        }

        return payment;
    }

    public async Task<bool> CancelPaymentAsync(string paymentId, string reason = "")
    {
        var payment = await _paymentRepository.GetByIdAsync(paymentId);
        if (payment == null)
            return false;

        // Can only cancel pending payments
        if (payment.Status != PaymentStatus.Pending)
            return false;

        payment.Status = PaymentStatus.Cancelled;
        payment.UpdatedAt = DateTime.UtcNow;
        await _paymentRepository.UpdateAsync(payment);

        _logger.LogInformation("Payment cancelled: {PaymentId}, Reason: {Reason}", paymentId, reason);
        return true;
    }

    public async Task UpdatePaymentStatusAsync(string paymentId, PaymentStatus status)
    {
        var payment = await _paymentRepository.GetByIdAsync(paymentId);
        if (payment == null)
            return;

        payment.Status = status;
        payment.UpdatedAt = DateTime.UtcNow;

        if (status == PaymentStatus.Completed)
            payment.CompletedAt = DateTime.UtcNow;

        await _paymentRepository.UpdateAsync(payment);
    }

    public async Task<IEnumerable<Payment>> GetMerchantPaymentsAsync(string merchantId, int page = 1, int pageSize = 20)
    {
        return await _paymentRepository.GetByMerchantAsync(merchantId, page, pageSize);
    }
}

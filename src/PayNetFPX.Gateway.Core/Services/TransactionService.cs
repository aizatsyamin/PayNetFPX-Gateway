namespace PayNetFPX.Gateway.Core.Services;

using PayNetFPX.Gateway.Data.Entities;
using PayNetFPX.Gateway.Data.Repositories;

public interface ITransactionService
{
    Task<Transaction> CreateTransactionAsync(string paymentId, string bankCode, string? bankName, decimal amount);
    Task<Transaction?> GetTransactionAsync(string transactionId);
    Task<IEnumerable<Transaction>> GetPaymentTransactionsAsync(string paymentId);
    Task UpdateTransactionStatusAsync(string transactionId, TransactionStatus status, string? responseCode, string? responseMessage);
}

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(
        ITransactionRepository transactionRepository,
        ILogger<TransactionService> logger)
    {
        _transactionRepository = transactionRepository;
        _logger = logger;
    }

    public async Task<Transaction> CreateTransactionAsync(string paymentId, string bankCode, string? bankName, decimal amount)
    {
        var transaction = new Transaction
        {
            TransactionId = Guid.NewGuid().ToString(),
            PaymentId = paymentId,
            BankCode = bankCode,
            BankName = bankName,
            Amount = amount,
            Currency = "MYR",
            Status = TransactionStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _transactionRepository.AddAsync(transaction);

        _logger.LogInformation(
            "Transaction created: {TransactionId}, PaymentId: {PaymentId}, BankCode: {BankCode}",
            transaction.TransactionId, paymentId, bankCode);

        return transaction;
    }

    public async Task<Transaction?> GetTransactionAsync(string transactionId)
    {
        return await _transactionRepository.GetByIdAsync(transactionId);
    }

    public async Task<IEnumerable<Transaction>> GetPaymentTransactionsAsync(string paymentId)
    {
        return await _transactionRepository.GetByPaymentAsync(paymentId);
    }

    public async Task UpdateTransactionStatusAsync(string transactionId, TransactionStatus status, string? responseCode, string? responseMessage)
    {
        var transaction = await _transactionRepository.GetByIdAsync(transactionId);
        if (transaction == null)
            return;

        transaction.Status = status;
        transaction.ResponseCode = responseCode;
        transaction.ResponseMessage = responseMessage;

        if (status == TransactionStatus.Success)
            transaction.CompletedAt = DateTime.UtcNow;

        await _transactionRepository.UpdateAsync(transaction);

        _logger.LogInformation(
            "Transaction status updated: {TransactionId}, Status: {Status}",
            transactionId, status);
    }
}

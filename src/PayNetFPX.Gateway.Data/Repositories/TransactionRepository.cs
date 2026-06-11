namespace PayNetFPX.Gateway.Data.Repositories;

using PayNetFPX.Gateway.Data.Entities;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(string transactionId);
    Task<IEnumerable<Transaction>> GetByPaymentAsync(string paymentId);
    Task<Transaction?> GetByReferenceAsync(string referenceNumber);
    Task AddAsync(Transaction transaction);
    Task UpdateAsync(Transaction transaction);
}

public class TransactionRepository : ITransactionRepository
{
    private readonly PaymentDbContext _context;

    public TransactionRepository(PaymentDbContext context)
    {
        _context = context;
    }

    public async Task<Transaction?> GetByIdAsync(string transactionId)
    {
        return await _context.Transactions
            .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
    }

    public async Task<IEnumerable<Transaction>> GetByPaymentAsync(string paymentId)
    {
        return await _context.Transactions
            .Where(t => t.PaymentId == paymentId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<Transaction?> GetByReferenceAsync(string referenceNumber)
    {
        return await _context.Transactions
            .FirstOrDefaultAsync(t => t.ReferenceNumber == referenceNumber);
    }

    public async Task AddAsync(Transaction transaction)
    {
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Transaction transaction)
    {
        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
    }
}

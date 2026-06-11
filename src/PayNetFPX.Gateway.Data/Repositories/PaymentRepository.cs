namespace PayNetFPX.Gateway.Data.Repositories;

using PayNetFPX.Gateway.Data.Entities;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(string paymentId);
    Task<Payment?> GetByReferenceAsync(string merchantId, string reference);
    Task<IEnumerable<Payment>> GetByMerchantAsync(string merchantId, int pageNumber = 1, int pageSize = 20);
    Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status, int pageNumber = 1, int pageSize = 20);
    Task AddAsync(Payment payment);
    Task UpdateAsync(Payment payment);
    Task DeleteAsync(string paymentId);
    Task<int> GetCountByMerchantAsync(string merchantId);
}

public class PaymentRepository : IPaymentRepository
{
    private readonly PaymentDbContext _context;

    public PaymentRepository(PaymentDbContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByIdAsync(string paymentId)
    {
        return await _context.Payments
            .Include(p => p.Transactions)
            .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
    }

    public async Task<Payment?> GetByReferenceAsync(string merchantId, string reference)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(p => p.MerchantId == merchantId && p.Reference == reference);
    }

    public async Task<IEnumerable<Payment>> GetByMerchantAsync(string merchantId, int pageNumber = 1, int pageSize = 20)
    {
        return await _context.Payments
            .Where(p => p.MerchantId == merchantId)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status, int pageNumber = 1, int pageSize = 20)
    {
        return await _context.Payments
            .Where(p => p.Status == status)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task AddAsync(Payment payment)
    {
        await _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Payment payment)
    {
        _context.Payments.Update(payment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string paymentId)
    {
        var payment = await GetByIdAsync(paymentId);
        if (payment != null)
        {
            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetCountByMerchantAsync(string merchantId)
    {
        return await _context.Payments
            .CountAsync(p => p.MerchantId == merchantId);
    }
}

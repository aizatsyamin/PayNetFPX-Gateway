namespace PayNetFPX.Gateway.Data.Repositories;

using PayNetFPX.Gateway.Data.Entities;

public interface IMerchantRepository
{
    Task<Merchant?> GetByIdAsync(string merchantId);
    Task<Merchant?> GetByEmailAsync(string email);
    Task AddAsync(Merchant merchant);
    Task UpdateAsync(Merchant merchant);
    Task<bool> ExistsAsync(string merchantId);
}

public class MerchantRepository : IMerchantRepository
{
    private readonly PaymentDbContext _context;

    public MerchantRepository(PaymentDbContext context)
    {
        _context = context;
    }

    public async Task<Merchant?> GetByIdAsync(string merchantId)
    {
        return await _context.Merchants
            .Include(m => m.ApiKeys)
            .Include(m => m.Webhooks)
            .FirstOrDefaultAsync(m => m.MerchantId == merchantId);
    }

    public async Task<Merchant?> GetByEmailAsync(string email)
    {
        return await _context.Merchants
            .FirstOrDefaultAsync(m => m.Email == email);
    }

    public async Task AddAsync(Merchant merchant)
    {
        await _context.Merchants.AddAsync(merchant);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Merchant merchant)
    {
        _context.Merchants.Update(merchant);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(string merchantId)
    {
        return await _context.Merchants.AnyAsync(m => m.MerchantId == merchantId);
    }
}

namespace PayNetFPX.Gateway.Data.Repositories;

using PayNetFPX.Gateway.Data.Entities;

public interface IWebhookRepository
{
    Task<Webhook?> GetByIdAsync(string webhookId);
    Task<IEnumerable<Webhook>> GetByMerchantAsync(string merchantId);
    Task AddAsync(Webhook webhook);
    Task UpdateAsync(Webhook webhook);
    Task DeleteAsync(string webhookId);
}

public class WebhookRepository : IWebhookRepository
{
    private readonly PaymentDbContext _context;

    public WebhookRepository(PaymentDbContext context)
    {
        _context = context;
    }

    public async Task<Webhook?> GetByIdAsync(string webhookId)
    {
        return await _context.Webhooks
            .FirstOrDefaultAsync(w => w.WebhookId == webhookId);
    }

    public async Task<IEnumerable<Webhook>> GetByMerchantAsync(string merchantId)
    {
        return await _context.Webhooks
            .Where(w => w.MerchantId == merchantId)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(Webhook webhook)
    {
        await _context.Webhooks.AddAsync(webhook);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Webhook webhook)
    {
        _context.Webhooks.Update(webhook);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string webhookId)
    {
        var webhook = await GetByIdAsync(webhookId);
        if (webhook != null)
        {
            _context.Webhooks.Remove(webhook);
            await _context.SaveChangesAsync();
        }
    }
}

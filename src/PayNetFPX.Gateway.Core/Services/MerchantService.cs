namespace PayNetFPX.Gateway.Core.Services;

using PayNetFPX.Gateway.Data.Entities;
using PayNetFPX.Gateway.Data.Repositories;

public interface IMerchantService
{
    Task<Merchant> RegisterMerchantAsync(Merchant merchant);
    Task<Merchant?> GetMerchantAsync(string merchantId);
    Task UpdateMerchantAsync(Merchant merchant);
    Task<bool> MerchantExistsAsync(string merchantId);
}

public class MerchantService : IMerchantService
{
    private readonly IMerchantRepository _merchantRepository;
    private readonly ILogger<MerchantService> _logger;

    public MerchantService(
        IMerchantRepository merchantRepository,
        ILogger<MerchantService> logger)
    {
        _merchantRepository = merchantRepository;
        _logger = logger;
    }

    public async Task<Merchant> RegisterMerchantAsync(Merchant merchant)
    {
        // Check if email already exists
        var existing = await _merchantRepository.GetByEmailAsync(merchant.Email);
        if (existing != null)
            throw new InvalidOperationException($"Merchant with email {merchant.Email} already exists");

        merchant.MerchantId = Guid.NewGuid().ToString();
        merchant.CreatedAt = DateTime.UtcNow;
        merchant.UpdatedAt = DateTime.UtcNow;
        merchant.Status = MerchantStatus.Active;
        merchant.IsActive = true;

        await _merchantRepository.AddAsync(merchant);

        _logger.LogInformation("Merchant registered: {MerchantId}, Email: {Email}", 
            merchant.MerchantId, merchant.Email);

        return merchant;
    }

    public async Task<Merchant?> GetMerchantAsync(string merchantId)
    {
        return await _merchantRepository.GetByIdAsync(merchantId);
    }

    public async Task UpdateMerchantAsync(Merchant merchant)
    {
        merchant.UpdatedAt = DateTime.UtcNow;
        await _merchantRepository.UpdateAsync(merchant);

        _logger.LogInformation("Merchant updated: {MerchantId}", merchant.MerchantId);
    }

    public async Task<bool> MerchantExistsAsync(string merchantId)
    {
        return await _merchantRepository.ExistsAsync(merchantId);
    }
}

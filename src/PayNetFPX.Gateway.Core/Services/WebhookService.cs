namespace PayNetFPX.Gateway.Core.Services;

using PayNetFPX.Gateway.Data.Entities;
using PayNetFPX.Gateway.Data.Repositories;

public interface IWebhookService
{
    Task<Webhook> RegisterWebhookAsync(string merchantId, string url, string[] events, string? secret, string? description);
    Task<Webhook?> GetWebhookAsync(string webhookId);
    Task<IEnumerable<Webhook>> GetMerchantWebhooksAsync(string merchantId);
    Task UpdateWebhookAsync(Webhook webhook);
    Task DeleteWebhookAsync(string webhookId);
    Task TriggerWebhookAsync(Webhook webhook, string eventType, object eventData);
}

public class WebhookService : IWebhookService
{
    private readonly IWebhookRepository _webhookRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<WebhookService> _logger;

    public WebhookService(
        IWebhookRepository webhookRepository,
        IHttpClientFactory httpClientFactory,
        ILogger<WebhookService> logger)
    {
        _webhookRepository = webhookRepository;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<Webhook> RegisterWebhookAsync(string merchantId, string url, string[] events, string? secret, string? description)
    {
        var webhook = new Webhook
        {
            WebhookId = Guid.NewGuid().ToString(),
            MerchantId = merchantId,
            Url = url,
            Events = string.Join(",", events),
            Secret = secret,
            Description = description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _webhookRepository.AddAsync(webhook);

        _logger.LogInformation(
            "Webhook registered: {WebhookId}, MerchantId: {MerchantId}, Url: {Url}",
            webhook.WebhookId, merchantId, url);

        return webhook;
    }

    public async Task<Webhook?> GetWebhookAsync(string webhookId)
    {
        return await _webhookRepository.GetByIdAsync(webhookId);
    }

    public async Task<IEnumerable<Webhook>> GetMerchantWebhooksAsync(string merchantId)
    {
        return await _webhookRepository.GetByMerchantAsync(merchantId);
    }

    public async Task UpdateWebhookAsync(Webhook webhook)
    {
        webhook.UpdatedAt = DateTime.UtcNow;
        await _webhookRepository.UpdateAsync(webhook);
    }

    public async Task DeleteWebhookAsync(string webhookId)
    {
        await _webhookRepository.DeleteAsync(webhookId);
        _logger.LogInformation("Webhook deleted: {WebhookId}", webhookId);
    }

    public async Task TriggerWebhookAsync(Webhook webhook, string eventType, object eventData)
    {
        if (!webhook.IsActive)
            return;

        // Check if webhook should handle this event
        var events = webhook.Events.Split(',');
        if (!events.Contains(eventType))
            return;

        try
        {
            var client = _httpClientFactory.CreateClient();
            
            var payload = new
            {
                @event = eventType,
                timestamp = DateTime.UtcNow,
                data = eventData
            };

            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(payload),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(webhook.Url, content);

            if (response.IsSuccessStatusCode)
            {
                webhook.LastTriggeredAt = DateTime.UtcNow;
                webhook.FailureCount = 0;
                await _webhookRepository.UpdateAsync(webhook);

                _logger.LogInformation(
                    "Webhook triggered successfully: {WebhookId}, Event: {Event}",
                    webhook.WebhookId, eventType);
            }
            else
            {
                webhook.FailureCount++;
                await _webhookRepository.UpdateAsync(webhook);

                _logger.LogWarning(
                    "Webhook trigger failed: {WebhookId}, Event: {Event}, Status: {StatusCode}",
                    webhook.WebhookId, eventType, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            webhook.FailureCount++;
            await _webhookRepository.UpdateAsync(webhook);

            _logger.LogError(ex,
                "Error triggering webhook: {WebhookId}, Event: {Event}",
                webhook.WebhookId, eventType);
        }
    }
}

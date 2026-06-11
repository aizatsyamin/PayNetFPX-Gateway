namespace PayNetFPX.Gateway.Data;

using Microsoft.EntityFrameworkCore;
using PayNetFPX.Gateway.Data.Entities;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
    {
    }

    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;
    public DbSet<Merchant> Merchants { get; set; } = null!;
    public DbSet<ApiKey> ApiKeys { get; set; } = null!;
    public DbSet<Webhook> Webhooks { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Payment entity
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId);
            entity.Property(e => e.PaymentId).ValueGeneratedNever();
            entity.Property(e => e.MerchantId).IsRequired();
            entity.Property(e => e.Reference).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.Currency).HasMaxLength(3);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CustomerEmail).HasMaxLength(255);
            entity.Property(e => e.CustomerPhone).HasMaxLength(20);
            entity.Property(e => e.ReturnUrl).HasMaxLength(500);
            entity.Property(e => e.CallbackUrl).HasMaxLength(500);

            entity.HasIndex(e => e.MerchantId);
            entity.HasIndex(e => e.Reference);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);

            entity.HasMany(e => e.Transactions)
                .WithOne(t => t.Payment)
                .HasForeignKey(t => t.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Transaction entity
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId);
            entity.Property(e => e.TransactionId).ValueGeneratedNever();
            entity.Property(e => e.PaymentId).IsRequired();
            entity.Property(e => e.BankCode).IsRequired().HasMaxLength(10);
            entity.Property(e => e.BankName).HasMaxLength(100);
            entity.Property(e => e.ReferenceNumber).HasMaxLength(50);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.Currency).HasMaxLength(3);
            entity.Property(e => e.ResponseCode).HasMaxLength(10);
            entity.Property(e => e.ResponseMessage).HasMaxLength(500);

            entity.HasIndex(e => e.PaymentId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.ReferenceNumber);
            entity.HasIndex(e => e.CreatedAt);
        });

        // Configure Merchant entity
        modelBuilder.Entity<Merchant>(entity =>
        {
            entity.HasKey(e => e.MerchantId);
            entity.Property(e => e.MerchantId).ValueGeneratedNever();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.PostalCode).HasMaxLength(10);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.BusinessType).HasMaxLength(100);
            entity.Property(e => e.BusinessRegistration).HasMaxLength(50);

            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Status);

            entity.HasMany(e => e.ApiKeys)
                .WithOne(a => a.Merchant)
                .HasForeignKey(a => a.MerchantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Webhooks)
                .WithOne(w => w.Merchant)
                .HasForeignKey(w => w.MerchantId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure ApiKey entity
        modelBuilder.Entity<ApiKey>(entity =>
        {
            entity.HasKey(e => e.ApiKeyId);
            entity.Property(e => e.ApiKeyId).ValueGeneratedNever();
            entity.Property(e => e.MerchantId).IsRequired();
            entity.Property(e => e.Key).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Environment).HasMaxLength(50);

            entity.HasIndex(e => e.MerchantId);
            entity.HasIndex(e => e.Key).IsUnique();
        });

        // Configure Webhook entity
        modelBuilder.Entity<Webhook>(entity =>
        {
            entity.HasKey(e => e.WebhookId);
            entity.Property(e => e.WebhookId).ValueGeneratedNever();
            entity.Property(e => e.MerchantId).IsRequired();
            entity.Property(e => e.Url).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Secret).HasMaxLength(500);
            entity.Property(e => e.Events).IsRequired();

            entity.HasIndex(e => e.MerchantId);
            entity.HasIndex(e => e.IsActive);
        });
    }
}

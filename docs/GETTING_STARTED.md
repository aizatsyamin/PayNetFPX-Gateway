# Getting Started with PayNetFPX-Gateway

## Prerequisites

Before you begin, ensure you have the following installed:

- **ASP.NET 10 SDK** - [Download](https://dotnet.microsoft.com/download)
- **SQL Server 2019 or later** (or SQL Server LocalDB)
- **Git** - [Download](https://git-scm.com/)
- **Visual Studio 2022** or **Visual Studio Code** with C# extension
- **PayNet FPX Account** - Contact [PayNet](https://www.paynet.my/)

## Installation

### 1. Clone the Repository

```bash
git clone https://github.com/aizatsyamin/PayNetFPX-Gateway.git
cd PayNetFPX-Gateway
```

### 2. Install Dependencies

```bash
dotnet restore
```

### 3. Database Setup

#### Option A: Using SQL Server Management Studio

1. Open SQL Server Management Studio
2. Create a new database named `PayNetFPXGateway`
3. Run the migration script from `scripts/database/initial-schema.sql`

#### Option B: Using Entity Framework Core Migrations

```bash
dotnet ef database update
```

This will automatically create the database and apply all pending migrations.

### 4. Configure Application Settings

Create an `appsettings.Development.json` file in the root directory:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PayNetFPXGateway;Trusted_Connection=true;"
  },
  "PayNet": {
    "ApiUrl": "https://sandbox.paynet.my/api",
    "ApiKey": "your-paynet-api-key",
    "Secret": "your-paynet-secret",
    "MerchantId": "your-merchant-id",
    "SellerExchange": "your-seller-exchange-id"
  },
  "Jwt": {
    "Key": "your-jwt-secret-key-minimum-32-characters-long",
    "Issuer": "paynetfpx-gateway",
    "Audience": "paynetfpx-clients",
    "ExpirationMinutes": 60
  },
  "ApiKey": {
    "DefaultKey": "your-default-api-key"
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000", "http://localhost:4200"]
  }
}
```

### 5. Build the Application

```bash
dotnet build
```

### 6. Run the Application

```bash
dotnet run
```

The application will start at `https://localhost:5001`.

---

## Quick Start Guide

### Step 1: Register Your Merchant

```http
POST /api/v1/merchants/register
Content-Type: application/json

{
  "name": "Your Business Name",
  "email": "business@example.com",
  "phoneNumber": "60123456789",
  "address": "123 Business Street",
  "city": "Kuala Lumpur",
  "state": "WP",
  "postalCode": "50000",
  "country": "MY",
  "businessType": "E-commerce",
  "businessRegistration": "123456789012"
}
```

### Step 2: Generate API Key

After registration, generate an API key from the merchant dashboard or via API:

```http
POST /api/v1/merchants/api-keys
Authorization: Bearer your_jwt_token
Content-Type: application/json

{
  "name": "Production API Key",
  "environment": "production"
}
```

### Step 3: Create Your First Payment

```http
POST /api/v1/payments/initiate
Authorization: Bearer YOUR_API_KEY
Content-Type: application/json

{
  "merchantId": "MERCHANT001",
  "amount": 100.00,
  "currency": "MYR",
  "reference": "TEST-001",
  "description": "Test Payment",
  "customerEmail": "customer@example.com",
  "returnUrl": "https://yourapp.com/payment/success",
  "callbackUrl": "https://yourapp.com/webhook/payment"
}
```

### Step 4: Redirect Customer to Payment Gateway

The response will contain a `redirectUrl`. Redirect your customer to this URL to complete the payment.

### Step 5: Receive Payment Confirmation

Your webhook endpoint will receive a callback when the payment is completed:

```json
{
  "event": "payment.completed",
  "data": {
    "paymentId": "PAY-xxx",
    "status": "COMPLETED",
    "amount": 100.00
  }
}
```

---

## Development Workflow

### Folder Structure

```
PayNetFPX-Gateway/
├── src/
│   ├── PayNetFPX.Gateway.API/
│   │   ├── Controllers/
│   │   ├── Middleware/
│   │   ├── Startup.cs
│   │   └── Program.cs
│   ├── PayNetFPX.Gateway.Core/
│   │   ├── Services/
│   │   ├── Models/
│   │   └── Interfaces/
│   ├── PayNetFPX.Gateway.Data/
│   │   ├── Repositories/
│   │   ├── Context/
│   │   └── Entities/
│   └── PayNetFPX.Gateway.Integration/
│       ├── PayNet/
│       └── Clients/
├── tests/
│   ├── PayNetFPX.Gateway.Tests.Unit/
│   ├── PayNetFPX.Gateway.Tests.Integration/
│   └── PayNetFPX.Gateway.Tests.E2E/
├── docs/
├── scripts/
└── docker-compose.yml
```

### Running Tests

#### Unit Tests

```bash
dotnet test PayNetFPX.Gateway.Tests.Unit
```

#### Integration Tests

```bash
dotnet test PayNetFPX.Gateway.Tests.Integration
```

#### All Tests

```bash
dotnet test
```

### Code Quality Checks

```bash
# Run static analysis
dotnet analyzers

# Format code
dotnet format

# Run code coverage
dotnet test /p:CollectCoverage=true
```

---

## Docker Setup

### Build Docker Image

```bash
docker build -t paynetfpx-gateway:latest .
```

### Run with Docker Compose

```bash
docker-compose up -d
```

This will start:
- ASP.NET 10 API (port 5001)
- SQL Server (port 1433)
- Redis Cache (port 6379) [optional]

### Docker Compose File Example

```yaml
version: '3.8'

services:
  api:
    build: .
    ports:
      - "5001:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=PayNetFPXGateway;User Id=sa;Password=YourPassword123!;
    depends_on:
      - sqlserver

  sqlserver:
    image: mcr.microsoft.com/mssql/server:2019-latest
    ports:
      - "1433:1433"
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourPassword123!
    volumes:
      - sqlserver-data:/var/opt/mssql

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"

volumes:
  sqlserver-data:
```

---

## Configuration Guide

### PayNet Integration

Update the `appsettings.json` with your PayNet credentials:

```json
"PayNet": {
  "ApiUrl": "https://api.paynet.my",
  "ApiKey": "your-api-key",
  "Secret": "your-secret-key",
  "MerchantId": "your-merchant-id",
  "Timeout": 30
}
```

### Database Configuration

Customize database connection:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=your-server;Database=PayNetFPXGateway;User Id=sa;Password=your-password;"
}
```

### JWT Configuration

Configure JWT authentication:

```json
"Jwt": {
  "Key": "your-secret-key-at-least-32-characters-long",
  "Issuer": "paynetfpx-gateway",
  "Audience": "paynetfpx-clients",
  "ExpirationMinutes": 60,
  "RefreshTokenExpirationDays": 7
}
```

### Logging Configuration

Configure logging levels:

```json
"Logging": {
  "LogLevel": {
    "Default": "Information",
    "Microsoft": "Warning",
    "PayNetFPX.Gateway": "Debug"
  },
  "Console": {
    "IncludeScopes": true,
    "TimestampFormat": "yyyy-MM-dd HH:mm:ss"
  }
}
```

---

## Testing the API

### Using Postman

1. Import the Postman collection: `docs/postman-collection.json`
2. Set the base URL: `https://localhost:5001/api/v1`
3. Set the Authorization header with your API key
4. Test endpoints

### Using cURL

```bash
# Initiate payment
curl -X POST https://localhost:5001/api/v1/payments/initiate \
  -H "Authorization: Bearer YOUR_API_KEY" \
  -H "Content-Type: application/json" \
  -d '{
    "merchantId": "MERCHANT001",
    "amount": 100.00,
    "currency": "MYR",
    "reference": "TEST-001",
    "description": "Test Payment",
    "customerEmail": "test@example.com",
    "returnUrl": "https://localhost:3000/success",
    "callbackUrl": "https://localhost:5001/webhook/payment"
  }'
```

### Using Swagger/OpenAPI

Navigate to `https://localhost:5001/swagger` to access the interactive API documentation.

---

## Common Issues & Troubleshooting

### Issue: Database Connection Failed

**Solution:**
```bash
# Check SQL Server is running
sqlcmd -S localhost -U sa

# Update connection string in appsettings.json
# Verify server name, database name, and credentials
```

### Issue: PayNet API Authentication Failed

**Solution:**
- Verify API key and secret in `appsettings.json`
- Ensure you're using the correct PayNet API URL (sandbox vs production)
- Check PayNet firewall whitelist for your IP

### Issue: Migrations Failed

**Solution:**
```bash
# Remove last migration
dotnet ef migrations remove

# Apply migrations fresh
dotnet ef database drop --force
dotnet ef database update
```

### Issue: Port Already in Use

**Solution:**
```bash
# Find process using port 5001
netstat -ano | findstr :5001

# Kill the process
taskkill /PID <PID> /F

# Or use a different port
dotnet run --launch-profile "https" --project-port 5002
```

---

## Next Steps

1. **Read the [API Documentation](./API.md)** for detailed endpoint information
2. **Review the [Architecture Guide](./ARCHITECTURE.md)** to understand system design
3. **Check the [Deployment Guide](./DEPLOYMENT.md)** for production setup
4. **Explore [Examples](./EXAMPLES.md)** for code samples
5. **Review [Security Best Practices](./SECURITY.md)**

---

## Additional Resources

- [PayNet FPX Documentation](https://www.paynet.my/)
- [ASP.NET Core Documentation](https://learn.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [REST API Best Practices](https://restfulapi.net/)

## Getting Help

- 📖 Check the documentation files in the `docs/` folder
- 🐛 Report bugs via [GitHub Issues](https://github.com/aizatsyamin/PayNetFPX-Gateway/issues)
- 💬 Discuss features via [GitHub Discussions](https://github.com/aizatsyamin/PayNetFPX-Gateway/discussions)
- 📧 Contact support at support@paynetfpx.com

---

**Happy coding! 🚀**

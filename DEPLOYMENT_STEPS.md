# 🚀 Render + Azure SQL Deployment Guide

## 🎯 Step-by-Step Deployment với SQL Server

### 🗄️ STEP 1: Setup Azure SQL Database

1. **Tạo Azure SQL Database:**
   ```
   Portal: https://portal.azure.com
   → Create a resource → SQL Database
   → Database name: FinancialAppDB
   → Server: Create new
   → Pricing tier: Basic (5 DTU) - $4.90/month
   ```

2. **Configure Firewall Rules:**
   ```
   SQL Database → Networking → Firewall rules
   → Add current client IP address
   → Allow Azure services and resources: YES
   ```

3. **Get Connection String:**
   ```
   SQL Database → Connection strings → ADO.NET
   Copy the connection string và replace {your_password}
   ```

### 🌐 STEP 2: Deploy Backend trên Render

1. **Tạo Web Service:**
   ```
   Render.com → New → Web Service
   → Connect GitHub: ThanhTaiNguyen04/FinancialApp-CleanArchitecture
   → Name: financialapp-api
   → Branch: main
   ```

2. **Build Configuration:**
   ```
   Build Command: chmod +x render-build.sh && ./render-build.sh
   Start Command: chmod +x render-start.sh && ./render-start.sh
   ```

3. **Environment Variables:**
   ```
   ASPNETCORE_ENVIRONMENT=Production
   ASPNETCORE_URLS=http://0.0.0.0:$PORT
   
   ConnectionStrings__DefaultConnection=Server=tcp:YOUR_SERVER.database.windows.net,1433;Initial Catalog=FinancialAppDB;Persist Security Info=False;User ID=YOUR_USERNAME;Password=YOUR_PASSWORD;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
   
   JWT__SecretKey=MyVerySecretKeyForFinancialAppThatIsAtLeast32CharactersForProduction!
   JWT__Issuer=FinancialApp
   JWT__Audience=FinancialAppUsers
   JWT__ExpirationHours=24
   ```

### 📱 STEP 3: Update Mobile App cho Production

Update `constants.js`:
```javascript
export const API_BASE_URL = __DEV__ 
  ? 'http://10.0.2.2:50255'  // Development
  : 'https://financialapp-api.onrender.com';  // Production URL từ Render
```

### 🔧 STEP 4: Database Migration

Sau khi deploy thành công, database sẽ được tạo tự động thông qua:
```csharp
// Trong Program.cs
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated(); // Tạo tables tự động
}
```

### ✅ STEP 5: Test Production API

1. **Health Check:**
   ```
   GET https://financialapp-api.onrender.com/health
   Expected: {"status":"healthy","timestamp":"...","environment":"Production"}
   ```

2. **Swagger UI:**
   ```
   https://financialapp-api.onrender.com/swagger
   ```

3. **Test Authentication:**
   ```
   POST https://financialapp-api.onrender.com/api/auth/register
   Body: {
     "email": "test@example.com",
     "password": "Test123!",
     "fullName": "Test User"
   }
   ```

## 💰 Cost Estimate:

- **Render Web Service**: $7/month (Starter plan)
- **Azure SQL Database**: $4.90/month (Basic tier)
- **Total**: ~$12/month cho production setup

## 📊 Expected Performance:

- **Cold Start**: 5-10 seconds (first request)
- **Response Time**: 100-300ms after warm-up
- **Uptime**: 99.9% SLA
- **Database Connections**: Pooled và optimized

## 🚨 Production Checklist:

✅ HTTPS enabled (automatic on Render)
✅ Environment variables secured
✅ Database firewall configured
✅ CORS configured cho frontend domains
✅ Logging enabled cho debugging
✅ Health checks active
✅ Auto-deployment từ GitHub

**Ready to go live! 🔥**
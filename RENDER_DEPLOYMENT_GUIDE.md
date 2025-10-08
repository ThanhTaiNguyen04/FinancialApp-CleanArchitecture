# Render.com Deployment Configuration for FinancialApp

## 📋 Deploy Steps on Render:

### 1. 🔗 Connect GitHub Repository
- Vào [Render.com](https://render.com)
- Sign up/Login với GitHub account
- Click "New" → "Web Service" 
- Connect GitHub repository: `ThanhTaiNguyen04/FinancialApp-CleanArchitecture`

### 2. ⚙️ Web Service Configuration

**Basic Settings:**
```
Name: financialapp-api
Environment: Native Environment (.NET)
Branch: main
Root Directory: . (leave blank)
```

**Build & Deploy Settings:**
```
Build Command: chmod +x render-build.sh && ./render-build.sh
Start Command: chmod +x render-start.sh && ./render-start.sh
```

**Environment Variables:**
```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:$PORT
ConnectionStrings__DefaultConnection=Server=tcp:your-server.database.windows.net,1433;Initial Catalog=FinancialAppDB;Persist Security Info=False;User ID=your-username;Password=your-password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
JWT__SecretKey=MyVerySecretKeyForFinancialAppThatIsAtLeast32CharactersForProduction!
JWT__Issuer=FinancialApp
JWT__Audience=FinancialAppUsers  
JWT__ExpirationHours=24
```

### 3. 🗄️ SQL Server Database Setup

**Option A: Azure SQL Database** ✅ (Recommended)
1. **Tạo Azure SQL Database:**
   - Vào [Azure Portal](https://portal.azure.com)
   - Create → SQL Database
   - Tên: `FinancialAppDB`
   - Server: Tạo mới hoặc dùng existing
   - Pricing tier: Basic (5 DTU) cho demo

2. **Get Connection String:**
   ```
   Server=tcp:your-server.database.windows.net,1433;Initial Catalog=FinancialAppDB;Persist Security Info=False;User ID=your-username;Password=your-password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
   ```

**Option B: Other SQL Server Providers:**
- **Azure SQL Managed Instance** (Enterprise)
- **AWS RDS for SQL Server** 
- **Google Cloud SQL Server**
- **ElephantSQL** (PostgreSQL alternative)

### 4. 🚀 Advanced Configuration

**Dockerfile (Alternative approach):**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY FinancialApp.Presentation/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "FinancialApp.Presentation.dll"]
```

**Health Check Endpoint:**
Add to `Program.cs`:
```csharp
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));
```

### 5. 🔧 Mobile App Configuration

Update mobile constants for production:
```javascript
export const API_BASE_URL = __DEV__ 
  ? 'http://10.0.2.2:50255'
  : 'https://your-render-app.onrender.com';  // Replace with your Render URL
```

### 6. 📊 Monitoring & Logs

- **Logs**: Available in Render dashboard
- **Metrics**: CPU, Memory usage tracking
- **Health**: Automatic health checks
- **SSL**: Free SSL certificates included

## 🎯 Expected Results:

✅ **API Base URL**: `https://financialapp-api.onrender.com`
✅ **Swagger UI**: `https://financialapp-api.onrender.com/swagger`
✅ **Health Check**: `https://financialapp-api.onrender.com/health`
✅ **Auto SSL**: HTTPS enabled by default
✅ **Auto Deploy**: Deploys automatically on git push

## 🚨 Important Notes:

1. **Free Tier Limitations:**
   - App sleeps after 15 minutes of inactivity
   - 750 hours/month limit
   - Slower cold start times

2. **Database:**
   - SQLite file sẽ bị reset khi app restart trên free tier
   - Consider PostgreSQL addon cho persistent data

3. **Environment:**
   - Production environment variables
   - CORS configured cho frontend domains
   - Logging level set to Information

## 🔄 CI/CD Flow:

```
Git Push → GitHub → Render Auto Deploy → Live API
```

**Deployment time: ~3-5 minutes** ⚡
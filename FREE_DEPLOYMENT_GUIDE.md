# 🆓 FREE Render Deployment với SQL Server In-Memory

## 🎯 Mục tiêu: Deploy backend miễn phí để test với Expo Go

### ✅ Configuration đã setup:

**Development (Local):**
- SQL Server với connection string từ appsettings.json
- Port 50255 để connect với mobile app

**Production (Render - FREE):**
- SQL Server LocalDB in-memory (không cần external database)
- Port dynamic từ Render ($PORT)
- Database tự động tạo khi app start

### 📋 DEPLOY STEPS:

#### 1. 🔗 Vào Render.com
- Login với GitHub account
- New → Web Service
- Repository: `ThanhTaiNguyen04/FinancialApp-CleanArchitecture`

#### 2. ⚙️ Service Configuration
```
Name: financialapp-api
Branch: main
Environment: Native Environment (.NET)
Build Command: chmod +x render-build.sh && ./render-build.sh
Start Command: chmod +x render-start.sh && ./render-start.sh
```

#### 3. 🔧 Environment Variables (MINIMAL - FREE)
```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:$PORT
JWT__SecretKey=MyVerySecretKeyForFinancialAppThatIsAtLeast32CharactersForProduction!
JWT__Issuer=FinancialApp  
JWT__Audience=FinancialAppUsers
JWT__ExpirationHours=24
```

**💡 Không cần database connection string!**

#### 4. 🚀 Deploy & Test

Sau khi deploy xong (~5 phút):
1. **Health Check:** `https://your-app.onrender.com/health`
2. **Swagger UI:** `https://your-app.onrender.com/swagger`
3. **Test API:** Register user → Login → Get transactions

#### 5. 📱 Update Mobile App cho Expo Go

Update `FinancialAppMobile/src/utils/constants.js`:
```javascript
export const API_BASE_URL = __DEV__ 
  ? 'http://10.0.2.2:50255'  // Local development
  : 'https://YOUR_RENDER_APP_NAME.onrender.com';  // Production Render URL
```

### 🎯 Testing với Expo Go:

1. **Update constants.js** với Render URL
2. **Start Expo:** `npx expo start`
3. **Scan QR code** với Expo Go app
4. **Test registration & login**

### ⚠️ Limitations (FREE Tier):

- **Database**: In-memory, data mất khi app restart
- **Sleep**: App ngủ sau 15 phút không hoạt động  
- **Cold start**: 10-30 giây để wake up
- **Uptime**: 750 hours/tháng

### 🔥 Advantages:

✅ **$0 cost** - Hoàn toàn miễn phí
✅ **No external dependencies** - Không cần Azure/AWS
✅ **Auto SSL** - HTTPS automatically
✅ **Auto deployment** - Tự động deploy khi git push
✅ **Full API functionality** - Tất cả endpoints hoạt động
✅ **Perfect for demos** - Ideal cho testing và presentation

### 📊 Expected URLs:

- **API Base**: `https://financialapp-api.onrender.com`
- **Swagger**: `https://financialapp-api.onrender.com/swagger`  
- **Health**: `https://financialapp-api.onrender.com/health`
- **Auth**: `https://financialapp-api.onrender.com/api/auth/login`
- **Transactions**: `https://financialapp-api.onrender.com/api/transactions`

## 🚀 Ready to deploy với $0 cost!

**Perfect solution cho testing Expo Go app với real backend API! 🎉**
# 🆓 FREE Database Alternatives for Render Deployment

## 1. 🐘 Supabase PostgreSQL (FREE - Recommended)

**Setup:**
1. Vào [supabase.com](https://supabase.com)
2. Create new project → Get database URL
3. Install PostgreSQL EF Core:
   ```bash
   dotnet add FinancialApp.Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL
   ```

**Update Program.cs:**
```csharp
// Replace SQL Server with PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
```

**Connection String:**
```
ConnectionStrings__DefaultConnection=postgresql://postgres:[YOUR-PASSWORD]@db.[YOUR-REF].supabase.co:5432/postgres
```

## 2. 🚂 Railway PostgreSQL (FREE)

**Setup:**
1. Vào [railway.app](https://railway.app)
2. New Project → Add PostgreSQL
3. Copy connection string

**Connection String:**
```
ConnectionStrings__DefaultConnection=postgresql://postgres:password@host:port/railway
```

## 3. 🌍 PlanetScale MySQL (FREE)

**Setup:**
1. Vào [planetscale.com](https://planetscale.com)
2. Create database → Get connection string
3. Install MySQL EF Core:
   ```bash
   dotnet add FinancialApp.Infrastructure package Pomelo.EntityFrameworkCore.MySql
   ```

**Update Program.cs:**
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
```

## 4. 💾 SQLite (File-based - Simplest)

**For demo purposes only:**
```csharp
// In Program.cs
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=financialapp.db"));
```

**Pros:** No external database needed
**Cons:** Data bị mất khi app restart trên Render free tier

---

## 🎯 RECOMMENDED: Supabase PostgreSQL

**Why?**
- ✅ 500MB storage miễn phí
- ✅ 2 database projects
- ✅ Persistent data 
- ✅ Real-time features
- ✅ Built-in auth (có thể dùng thay JWT)
- ✅ Dashboard để view data

**Setup Steps:**
1. Create Supabase project
2. Install `Npgsql.EntityFrameworkCore.PostgreSQL`
3. Update connection string
4. Deploy to Render
5. Database migrations sẽ chạy tự động

**Total Cost: $0 🎉**
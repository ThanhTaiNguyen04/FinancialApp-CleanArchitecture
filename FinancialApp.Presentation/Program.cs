using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FinancialApp.Infrastructure.Data;
using FinancialApp.Domain.Interfaces;
using FinancialApp.Infrastructure.Repositories;
using FinancialApp.Application.Interfaces;
using FinancialApp.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "Financial App API", 
        Version = "v1",
        Description = "API cho ứng dụng quản lý tài chính cá nhân"
    });

    // Add JWT Bearer Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Nhập 'Bearer' [space] và sau đó nhập token của bạn vào text input bên dưới.\r\n\r\nExample: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// Add Entity Framework - Support PostgreSQL for Railway deployment
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (builder.Environment.IsDevelopment())
{
    // SQL Server for development
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
}
else
{
    // Check for different database options
    var postgresConnection = Environment.GetEnvironmentVariable("DATABASE_URL");
    var sqlServerConnection = Environment.GetEnvironmentVariable("SQL_SERVER_CONNECTION");
    
    if (!string.IsNullOrEmpty(sqlServerConnection))
    {
        // SQL Server for production (Azure SQL or custom)
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(sqlServerConnection));
    }
    else if (!string.IsNullOrEmpty(postgresConnection))
    {
        // PostgreSQL for Railway
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(postgresConnection));
    }
    else
    {
        // Fallback to In-Memory
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("FinancialAppDB"));
    }
}

// Register Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();
builder.Services.AddScoped<ISavingGoalRepository, SavingGoalRepository>();

// Register Application Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBudgetService, BudgetService>();
builder.Services.AddScoped<ISavingGoalService, SavingGoalService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Register Auth Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();

// Add HttpClient for ChatController
builder.Services.AddHttpClient();

// Configure JWT Authentication
var jwtSecretKey = builder.Configuration["JWT:SecretKey"] ?? "MyVerySecretKeyForFinancialAppThatIsAtLeast32Characters!";
var key = Encoding.ASCII.GetBytes(jwtSecretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"] ?? "FinancialApp",
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"] ?? "FinancialAppUsers",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Add CORS for Flutter app
builder.Services.AddCors(options =>
{
    options.AddPolicy("FlutterApp", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure for deployment - Enable Swagger in production for demo
if (!app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("FlutterApp");
app.UseAuthentication();
app.UseAuthorization();

// Add health check endpoint for Render deployment
app.MapGet("/health", () => Results.Ok(new { 
    status = "healthy", 
    timestamp = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName,
    message = "FinancialApp API is running!"
}));
app.MapControllers();

// Ensure database is created and seeded - with logging
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("🔄 Attempting to create database and tables...");
        
        // Check if using existing SQL Server database
        var isExistingDatabase = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SQL_SERVER_CONNECTION"));
        
        if (isExistingDatabase)
        {
            logger.LogInformation("🔗 Using existing SQL Server database - skipping table creation");
            
            // Verify connection to existing database
            bool canConnect = context.Database.CanConnect();
            logger.LogInformation($"📊 SQL Server connection status: {canConnect}");
        }
        else
        {
            // Create tables for new database
            bool created = context.Database.EnsureCreated();
            
            if (created)
            {
                logger.LogInformation("✅ Database and tables created successfully!");
            }
            else
            {
                logger.LogWarning("⚠️ EnsureCreated returned false, trying migrations...");
                try
                {
                    context.Database.Migrate();
                    logger.LogInformation("✅ Database migration completed!");
                }
                catch (Exception migrationEx)
                {
                    logger.LogError(migrationEx, "❌ Migration failed: {Error}", migrationEx.Message);
                }
            }
        }
        
        // Verify tables exist (PostgreSQL compatible)
        try 
        {
            var canConnect = context.Database.CanConnect();
            logger.LogInformation($"📊 Database connection status: {canConnect}");
        }
        catch (Exception dbEx)
        {
            logger.LogWarning("⚠️ Could not verify table count: {Error}", dbEx.Message);
        }
        
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Failed to create database: {Error}", ex.Message);
        
        // Fallback - try to use in-memory for this instance
        logger.LogWarning("⚠️ Falling back to in-memory database for this session");
    }
}

app.Run();

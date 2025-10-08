<<<<<<< HEAD
=======
<<<<<<< HEAD
# FinancialApp-CleanArchitecture
=======
>>>>>>> 8061604
# FinancialApp - Clean Architecture

Dự án FinancialApp được xây dựng theo kiến trúc Clean Architecture với 4 projects riêng biệt.

## 📁 Cấu trúc dự án

```
FinancialApp-CleanArchitecture/
├── FinancialApp.sln                    # Solution file
├── FinancialApp.Domain/                # 🏛️ Domain Layer (Core Business Logic)
│   ├── Entities/                       # Business entities
│   │   ├── User.cs
│   │   ├── Transaction.cs
│   │   ├── Contact.cs
│   │   └── Transfer.cs
│   └── Interfaces/                     # Repository contracts
│       ├── IUserRepository.cs
│       ├── ITransactionRepository.cs
│       ├── IContactRepository.cs
│       └── ITransferRepository.cs
│
├── FinancialApp.Application/           # 🎯 Application Layer (Use Cases)
│   ├── DTOs/                          # Data Transfer Objects
│   │   ├── UserDto.cs
│   │   └── TransactionDto.cs
│   ├── Interfaces/                    # Service contracts
│   │   └── IUserService.cs
│   └── Services/                      # Business logic implementation
│       └── UserService.cs
│
├── FinancialApp.Infrastructure/        # 🔧 Infrastructure Layer (Data Access)
│   ├── Data/
│   │   └── ApplicationDbContext.cs    # Entity Framework DbContext
│   └── Repositories/                  # Repository implementations
│       ├── UserRepository.cs
│       └── TransactionRepository.cs
│
└── FinancialApp.Presentation/          # 🌐 Presentation Layer (Web API)
    ├── Controllers/
    │   └── UsersController.cs         # API endpoints
    ├── Program.cs                     # Application entry point
    └── appsettings.json              # Configuration
```

## 🏗️ Kiến trúc Clean Architecture

### Dependency Flow (Luồng phụ thuộc):
```
Presentation → Application → Domain
Infrastructure → Domain
```

### Các tầng và trách nhiệm:

#### 1. **Domain Layer** (Tầng miền)
- ✅ **Mục đích**: Core business logic và entities
- ✅ **Nội dung**: Entities, Repository interfaces
- ✅ **Dependency**: Không phụ thuộc vào tầng nào khác (Independent)
- ✅ **Nguyên tắc**: Pure business logic, không có framework dependencies

#### 2. **Application Layer** (Tầng ứng dụng) 
- ✅ **Mục đích**: Use cases và orchestration
- ✅ **Nội dung**: DTOs, Service interfaces, Business workflows
- ✅ **Dependency**: Chỉ phụ thuộc vào Domain layer
- ✅ **Nguyên tắc**: Contain business rules specific to the application

#### 3. **Infrastructure Layer** (Tầng hạ tầng)
- ✅ **Mục đích**: Data access và external concerns
- ✅ **Nội dung**: Entity Framework, Repository implementations, External APIs
- ✅ **Dependency**: Phụ thuộc vào Domain layer
- ✅ **Nguyên tắc**: Implement domain interfaces

#### 4. **Presentation Layer** (Tầng trình bày)
- ✅ **Mục đích**: Handle user interactions (Web API)
- ✅ **Nội dung**: Controllers, API endpoints, Input validation
- ✅ **Dependency**: Phụ thuộc vào Application và Infrastructure layers
- ✅ **Nguyên tắc**: Thin layer, delegate to application services

## 🚀 Cách chạy dự án

1. **Điều kiện tiên quyết:**
   - .NET 9.0 SDK
   - SQL Server LocalDB

2. **Chạy ứng dụng:**
   ```bash
   cd FinancialApp-CleanArchitecture/FinancialApp.Presentation
   dotnet run
   ```

3. **Truy cập API:**
   - API Base URL: `http://localhost:5205`
   - Swagger UI: `http://localhost:5205/swagger`

## 📋 API Endpoints

| Endpoint | Method | Description |
|----------|---------|-------------|
| `/api/users/profile` | GET | Lấy thông tin profile user |
| `/api/users/financial-summary` | GET | Lấy tổng quan tài chính |
| `/api/users` | GET | Lấy danh sách users |
| `/api/users/{id}` | GET | Lấy user theo ID |
| `/api/users` | POST | Tạo user mới |
| `/api/users/{id}` | PUT | Cập nhật user |
| `/api/users/{id}` | DELETE | Xóa user |

## 🎯 Lợi ích của Clean Architecture

✅ **Testability**: Dễ dàng unit test từng layer  
✅ **Maintainability**: Code có cấu trúc rõ ràng, dễ bảo trì  
✅ **Flexibility**: Có thể thay đổi UI, database mà không ảnh hưởng business logic  
✅ **Separation of Concerns**: Mỗi layer có trách nhiệm riêng biệt  
✅ **Dependency Rule**: Dependencies chỉ point inward  
✅ **Framework Independence**: Business logic không phụ thuộc vào framework  

## 🔄 Luồng hoạt động

```
HTTP Request → Controller → Service → Repository → Database
                   ↓           ↓          ↓
            (Presentation) (Application) (Infrastructure)
                   ↓           ↓          ↓
              Input/Output ← DTOs ← Domain Entities
```

## 📦 Dependencies

### Domain Project
- Không có external dependencies (Pure .NET)

### Application Project  
- FinancialApp.Domain

### Infrastructure Project
- FinancialApp.Domain
- Microsoft.EntityFrameworkCore.SqlServer

### Presentation Project
- FinancialApp.Application  
- FinancialApp.Infrastructure
- Swashbuckle.AspNetCore (Swagger)

---

<<<<<<< HEAD
🎉 **Dự án đã sẵn sàng cho development và production!**
=======
🎉 **Dự án đã sẵn sàng cho development và production!**
>>>>>>> 2b2192e (feat: Fix data isolation and implement secure transaction API - Add [Authorize] attribute to TransactionsController - Fix GetAllTransactions to only return current user's data - Add JWT token validation and user ID extraction - Implement security checks for all CRUD operations - Add comprehensive logging for debugging and audit - Update port configuration to 50255 for mobile app compatibility)
>>>>>>> 8061604

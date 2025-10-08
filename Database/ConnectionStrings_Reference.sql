-- =============================================
-- FinancialApp Connection Strings Reference
-- Chọn connection string phù hợp với setup của bạn
-- =============================================

/* 
1. SQL Server Express (Local) - Windows Authentication
   Phù hợp cho: Development, máy cá nhân
*/
"Server=.\\SQLEXPRESS;Database=FinancialAppDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"

/* 
2. SQL Server Express (Local) - SQL Authentication
   Phù hợp cho: Khi có username/password
*/
"Server=.\\SQLEXPRESS;Database=FinancialAppDb;User Id=sa;Password=YourPassword;MultipleActiveResultSets=true;TrustServerCertificate=true"

/* 
3. SQL Server LocalDB (Default hiện tại)
   Phù hợp cho: Development nhanh
*/
"Server=(localdb)\\mssqllocaldb;Database=FinancialAppDb;Trusted_Connection=true;MultipleActiveResultSets=true"

/* 
4. SQL Server Full Instance
   Phù hợp cho: Production, server thật
*/
"Server=YOUR_SERVER_NAME;Database=FinancialAppDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"

/* 
5. SQL Server với IP Address
   Phù hợp cho: Remote server
*/
"Server=192.168.1.100,1433;Database=FinancialAppDb;User Id=sa;Password=YourPassword;MultipleActiveResultSets=true;TrustServerCertificate=true"

/* 
6. Azure SQL Database
   Phù hợp cho: Cloud deployment
*/
"Server=tcp:yourserver.database.windows.net,1433;Database=FinancialAppDb;User Id=yourusername;Password=yourpassword;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30"

-- =============================================
-- Hướng dẫn sử dụng:
-- 1. Copy connection string phù hợp
-- 2. Thay thế trong appsettings.json
-- 3. Chạy: dotnet ef database update
-- =============================================
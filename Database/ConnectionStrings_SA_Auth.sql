-- =============================================
-- FinancialApp Connection Strings - UPDATED
-- Sử dụng SQL Server Authentication
-- =============================================

/* 
HIỆN TẠI - SQL Server với SA Authentication
Phù hợp cho: Development và Production
*/
"Server=.\\MSSQLSERVER03;Database=FinancialAppDb;User Id=sa;Password=12345;MultipleActiveResultSets=true;TrustServerCertificate=true"

/* 
Các instance khác có thể sử dụng:
*/

-- MSSQLSERVER01 với SA
"Server=.\\MSSQLSERVER01;Database=FinancialAppDb;User Id=sa;Password=12345;MultipleActiveResultSets=true;TrustServerCertificate=true"

-- MSSQLSERVER02 với SA  
"Server=.\\MSSQLSERVER02;Database=FinancialAppDb;User Id=sa;Password=12345;MultipleActiveResultSets=true;TrustServerCertificate=true"

-- Default SQL Server instance với SA
"Server=localhost;Database=FinancialAppDb;User Id=sa;Password=12345;MultipleActiveResultSets=true;TrustServerCertificate=true"

-- Remote server với SA
"Server=192.168.1.100;Database=FinancialAppDb;User Id=sa;Password=12345;MultipleActiveResultSets=true;TrustServerCertificate=true"

/* 
Để test kết nối qua command line:
sqlcmd -S .\MSSQLSERVER03 -U sa -P 12345 -d FinancialAppDb
*/

-- =============================================
-- SECURITY NOTES:
-- 1. Trong production, sử dụng strong password
-- 2. Consider tạo dedicated database user thay vì sa
-- 3. Enable SSL encryption cho production
-- =============================================
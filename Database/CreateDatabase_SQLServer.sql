-- =============================================
-- FinancialApp SQL Server Setup Script
-- Chạy script này trong SQL Server Management Studio
-- =============================================

-- 1. Tạo Database
USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'FinancialAppDb')
BEGIN
    CREATE DATABASE FinancialAppDb
    ON (
        NAME = 'FinancialAppDb',
        FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\FinancialAppDb.mdf',
        SIZE = 100MB,
        FILEGROWTH = 10MB
    )
    LOG ON (
        NAME = 'FinancialAppDb_Log',
        FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\FinancialAppDb.ldf',
        SIZE = 10MB,
        FILEGROWTH = 10%
    );
    
    PRINT 'Database FinancialAppDb created successfully!';
END
ELSE
BEGIN
    PRINT 'Database FinancialAppDb already exists!';
END
GO

-- 2. Sử dụng database
USE FinancialAppDb;
GO

-- 3. Kiểm tra kết nối
SELECT 
    'Database: ' + DB_NAME() AS CurrentDatabase,
    'Server: ' + @@SERVERNAME AS ServerName,
    'Version: ' + @@VERSION AS SQLVersion,
    GETDATE() AS CurrentTime;

PRINT 'Ready for Entity Framework migrations!';
PRINT 'Run this command in Visual Studio terminal:';
PRINT 'dotnet ef database update --project FinancialApp.Infrastructure --startup-project FinancialApp.Presentation';
GO
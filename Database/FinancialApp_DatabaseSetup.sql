-- =============================================
-- FinancialApp Database Setup Script
-- Tạo database và tables cho ứng dụng quản lý tài chính
-- =============================================

-- Tạo Database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'FinancialAppDb')
BEGIN
    CREATE DATABASE FinancialAppDb;
END
GO

USE FinancialAppDb;
GO

-- =============================================
-- 1. TABLES CREATION
-- =============================================

-- Users Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
BEGIN
    CREATE TABLE Users (
        Id int IDENTITY(1,1) PRIMARY KEY,
        FullName nvarchar(100) NOT NULL,
        Email nvarchar(100) NOT NULL UNIQUE,
        Phone nvarchar(20) NOT NULL,
        AvatarUrl nvarchar(500) NOT NULL DEFAULT '',
        AvailableBalance decimal(18,2) NOT NULL DEFAULT 0.0,
        CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt datetime2 NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

-- Categories Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Categories' AND xtype='U')
BEGIN
    CREATE TABLE Categories (
        Id int IDENTITY(1,1) PRIMARY KEY,
        Name nvarchar(100) NOT NULL,
        IconName nvarchar(50) NOT NULL DEFAULT '',
        ColorCode nvarchar(20) NOT NULL DEFAULT '#000000',
        Type nvarchar(20) NOT NULL CHECK (Type IN ('income', 'expense')),
        IsActive bit NOT NULL DEFAULT 1,
        CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

-- Budgets Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Budgets' AND xtype='U')
BEGIN
    CREATE TABLE Budgets (
        Id int IDENTITY(1,1) PRIMARY KEY,
        UserId int NOT NULL,
        CategoryId int NOT NULL,
        BudgetAmount decimal(18,2) NOT NULL,
        SpentAmount decimal(18,2) NOT NULL DEFAULT 0.0,
        Month int NOT NULL CHECK (Month BETWEEN 1 AND 12),
        Year int NOT NULL,
        CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_Budgets_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
        CONSTRAINT FK_Budgets_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE CASCADE
    );
END
GO

-- SavingGoals Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SavingGoals' AND xtype='U')
BEGIN
    CREATE TABLE SavingGoals (
        Id int IDENTITY(1,1) PRIMARY KEY,
        UserId int NOT NULL,
        Name nvarchar(200) NOT NULL,
        Description nvarchar(500) NOT NULL DEFAULT '',
        TargetAmount decimal(18,2) NOT NULL,
        CurrentAmount decimal(18,2) NOT NULL DEFAULT 0.0,
        TargetDate datetime2 NOT NULL,
        IconName nvarchar(50) NOT NULL DEFAULT '',
        ColorCode nvarchar(20) NOT NULL DEFAULT '#000000',
        Status nvarchar(20) NOT NULL DEFAULT 'active' CHECK (Status IN ('active', 'completed', 'paused')),
        CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_SavingGoals_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
    );
END
GO

-- Transactions Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Transactions' AND xtype='U')
BEGIN
    CREATE TABLE Transactions (
        Id int IDENTITY(1,1) PRIMARY KEY,
        UserId int NOT NULL,
        Type nvarchar(20) NOT NULL CHECK (Type IN ('income', 'expense')),
        Category nvarchar(50) NOT NULL,
        Amount decimal(18,2) NOT NULL,
        Description nvarchar(200) NOT NULL,
        IconName nvarchar(50) NULL,
        TransactionDate datetime2 NOT NULL,
        CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
        CategoryId int NULL,
        CONSTRAINT FK_Transactions_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
        CONSTRAINT FK_Transactions_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
    );
END
GO

-- Contacts Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Contacts' AND xtype='U')
BEGIN
    CREATE TABLE Contacts (
        Id int IDENTITY(1,1) PRIMARY KEY,
        UserId int NOT NULL,
        FullName nvarchar(100) NOT NULL,
        Phone nvarchar(20) NOT NULL,
        Email nvarchar(100) NOT NULL,
        AvatarUrl nvarchar(500) NOT NULL DEFAULT '',
        IsRecent bit NOT NULL DEFAULT 0,
        LastTransactionDate datetime2 NULL,
        CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_Contacts_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
    );
END
GO

-- Transfers Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Transfers' AND xtype='U')
BEGIN
    CREATE TABLE Transfers (
        Id int IDENTITY(1,1) PRIMARY KEY,
        FromUserId int NOT NULL,
        ToContactId int NOT NULL,
        Amount decimal(18,2) NOT NULL,
        TransferType nvarchar(20) NOT NULL DEFAULT 'mobile',
        Description nvarchar(200) NOT NULL,
        Status nvarchar(20) NOT NULL DEFAULT 'pending' CHECK (Status IN ('pending', 'completed', 'failed')),
        TransferDate datetime2 NOT NULL DEFAULT GETUTCDATE(),
        CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_Transfers_FromUsers FOREIGN KEY (FromUserId) REFERENCES Users(Id) ON DELETE NO ACTION,
        CONSTRAINT FK_Transfers_ToContacts FOREIGN KEY (ToContactId) REFERENCES Contacts(Id) ON DELETE NO ACTION
    );
END
GO

-- =============================================
-- 2. INDEXES FOR PERFORMANCE
-- =============================================

-- Users indexes
IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IX_Users_Email')
    CREATE UNIQUE INDEX IX_Users_Email ON Users (Email);

-- Categories indexes  
IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IX_Categories_Type')
    CREATE INDEX IX_Categories_Type ON Categories (Type);

-- Budgets indexes
IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IX_Budgets_UserId')
    CREATE INDEX IX_Budgets_UserId ON Budgets (UserId);
IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IX_Budgets_CategoryId')
    CREATE INDEX IX_Budgets_CategoryId ON Budgets (CategoryId);
IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IX_Budgets_Month_Year')
    CREATE INDEX IX_Budgets_Month_Year ON Budgets (Month, Year);

-- SavingGoals indexes
IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IX_SavingGoals_UserId')
    CREATE INDEX IX_SavingGoals_UserId ON SavingGoals (UserId);
IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IX_SavingGoals_Status')
    CREATE INDEX IX_SavingGoals_Status ON SavingGoals (Status);

-- Transactions indexes
IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IX_Transactions_UserId_TransactionDate')
    CREATE INDEX IX_Transactions_UserId_TransactionDate ON Transactions (UserId, TransactionDate);
IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IX_Transactions_CategoryId')
    CREATE INDEX IX_Transactions_CategoryId ON Transactions (CategoryId);
IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IX_Transactions_Type')
    CREATE INDEX IX_Transactions_Type ON Transactions (Type);

-- Contacts indexes
IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IX_Contacts_UserId_Phone')
    CREATE UNIQUE INDEX IX_Contacts_UserId_Phone ON Contacts (UserId, Phone);

-- Transfers indexes
IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IX_Transfers_FromUserId_TransferDate')
    CREATE INDEX IX_Transfers_FromUserId_TransferDate ON Transfers (FromUserId, TransferDate);
IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IX_Transfers_ToContactId')
    CREATE INDEX IX_Transfers_ToContactId ON Transfers (ToContactId);

-- =============================================
-- 3. SAMPLE DATA (SEED DATA)
-- =============================================

-- Clear existing data (optional)
-- DELETE FROM Transfers;
-- DELETE FROM Transactions;
-- DELETE FROM Budgets;
-- DELETE FROM SavingGoals;
-- DELETE FROM Contacts;
-- DELETE FROM Categories;
-- DELETE FROM Users;

-- Reset identity seeds
-- DBCC CHECKIDENT ('Users', RESEED, 0);
-- DBCC CHECKIDENT ('Categories', RESEED, 0);
-- DBCC CHECKIDENT ('Budgets', RESEED, 0);
-- DBCC CHECKIDENT ('SavingGoals', RESEED, 0);
-- DBCC CHECKIDENT ('Transactions', RESEED, 0);
-- DBCC CHECKIDENT ('Contacts', RESEED, 0);
-- DBCC CHECKIDENT ('Transfers', RESEED, 0);

-- Insert Users
IF NOT EXISTS (SELECT 1 FROM Users WHERE Id = 1)
BEGIN
    SET IDENTITY_INSERT Users ON;
    INSERT INTO Users (Id, FullName, Email, Phone, AvatarUrl, AvailableBalance, CreatedAt, UpdatedAt)
    VALUES (1, N'Christopher Summers', N'christopher.summers@email.com', N'+1234567890', 
            N'https://images.pexels.com/photos/2167673/pexels-photo-2167673.jpeg?auto=compress&cs=tinysrgb&dpr=3&h=750&w=1260', 
            5320.50, '2025-09-26T10:00:00.0000000Z', '2025-09-26T10:00:00.0000000Z');
    SET IDENTITY_INSERT Users OFF;
END

-- Insert Categories
IF NOT EXISTS (SELECT 1 FROM Categories)
BEGIN
    SET IDENTITY_INSERT Categories ON;
    INSERT INTO Categories (Id, Name, IconName, ColorCode, Type, IsActive, CreatedAt) VALUES
    (1, N'Lương', N'salary', N'#4CAF50', N'income', 1, '2025-09-26T10:00:00.0000000Z'),
    (2, N'Đi chợ', N'shopping', N'#FF9800', N'expense', 1, '2025-09-26T10:00:00.0000000Z'),
    (3, N'Thuê nhà', N'home', N'#2196F3', N'expense', 1, '2025-09-26T10:00:00.0000000Z'),
    (4, N'Y tế', N'medical', N'#F44336', N'expense', 1, '2025-09-26T10:00:00.0000000Z'),
    (5, N'Du lịch', N'travel', N'#9C27B0', N'expense', 1, '2025-09-26T10:00:00.0000000Z');
    SET IDENTITY_INSERT Categories OFF;
END

-- Insert Budgets
IF NOT EXISTS (SELECT 1 FROM Budgets)
BEGIN
    SET IDENTITY_INSERT Budgets ON;
    INSERT INTO Budgets (Id, UserId, CategoryId, BudgetAmount, SpentAmount, Month, Year, CreatedAt, UpdatedAt) VALUES
    (1, 1, 2, 2000000.0, 455789.0, 9, 2025, '2025-09-26T10:00:00.0000000Z', '2025-09-26T10:00:00.0000000Z'),
    (2, 1, 3, 3500000.0, 3000000.0, 9, 2025, '2025-09-26T10:00:00.0000000Z', '2025-09-26T10:00:00.0000000Z'),
    (3, 1, 4, 1000000.0, 243789.0, 9, 2025, '2025-09-26T10:00:00.0000000Z', '2025-09-26T10:00:00.0000000Z');
    SET IDENTITY_INSERT Budgets OFF;
END

-- Insert SavingGoals
IF NOT EXISTS (SELECT 1 FROM SavingGoals)
BEGIN
    SET IDENTITY_INSERT SavingGoals ON;
    INSERT INTO SavingGoals (Id, UserId, Name, Description, TargetAmount, CurrentAmount, TargetDate, IconName, ColorCode, Status, CreatedAt, UpdatedAt) VALUES
    (1, 1, N'Giáo dục', N'Tiết kiệm cho việc học', 5000000.0, 2300000.0, '2026-03-26T10:00:00.0000000Z', N'education', N'#4CAF50', N'active', '2025-09-26T10:00:00.0000000Z', '2025-09-26T10:00:00.0000000Z'),
    (2, 1, N'Mua nhà', N'Tiết kiệm mua nhà', 47000000.0, 5600000.0, '2027-09-26T10:00:00.0000000Z', N'house', N'#FF9800', N'active', '2025-09-26T10:00:00.0000000Z', '2025-09-26T10:00:00.0000000Z'),
    (3, 1, N'Du lịch', N'Chuyến du lịch Hàn Quốc', 2200000.0, 800000.0, '2026-01-26T10:00:00.0000000Z', N'travel', N'#2196F3', N'active', '2025-09-26T10:00:00.0000000Z', '2025-09-26T10:00:00.0000000Z');
    SET IDENTITY_INSERT SavingGoals OFF;
END

-- Insert Transactions
IF NOT EXISTS (SELECT 1 FROM Transactions)
BEGIN
    SET IDENTITY_INSERT Transactions ON;
    INSERT INTO Transactions (Id, UserId, Type, Category, Amount, Description, IconName, TransactionDate, CreatedAt, CategoryId) VALUES
    (1, 1, N'income', N'Lương', 16000000.0, N'Lương tháng 9', NULL, '2025-09-21T10:00:00.0000000Z', '2025-09-26T10:00:00.0000000Z', NULL),
    (2, 1, N'expense', N'Đi chợ', 455789.0, N'Mua sắm hàng tuần', NULL, '2025-09-23T10:00:00.0000000Z', '2025-09-26T10:00:00.0000000Z', NULL),
    (3, 1, N'expense', N'Thuê nhà', 3000000.0, N'Tiền thuê nhà tháng 9', NULL, '2025-09-24T10:00:00.0000000Z', '2025-09-26T10:00:00.0000000Z', NULL),
    (4, 1, N'expense', N'Y tế', 243789.0, N'Khám bệnh và thuốc', NULL, '2025-09-25T10:00:00.0000000Z', '2025-09-26T10:00:00.0000000Z', NULL);
    SET IDENTITY_INSERT Transactions OFF;
END

-- Insert Contacts
IF NOT EXISTS (SELECT 1 FROM Contacts)
BEGIN
    SET IDENTITY_INSERT Contacts ON;
    INSERT INTO Contacts (Id, UserId, FullName, Phone, Email, AvatarUrl, IsRecent, LastTransactionDate, CreatedAt) VALUES
    (1, 1, N'Catherine Johnson', N'+1234567891', N'catherine@email.com', N'https://images.pexels.com/photos/733872/pexels-photo-733872.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=750&w=1260', 1, NULL, '2025-09-26T10:00:00.0000000Z'),
    (2, 1, N'Allan Smith', N'+1234567892', N'allan@email.com', N'https://images.pexels.com/photos/697509/pexels-photo-697509.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=750&w=1260', 1, NULL, '2025-09-26T10:00:00.0000000Z'),
    (3, 1, N'Kimberly Brown', N'+1234567893', N'kimberly@email.com', N'https://images.pexels.com/photos/2748091/pexels-photo-2748091.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=750&w=1260', 1, NULL, '2025-09-26T10:00:00.0000000Z');
    SET IDENTITY_INSERT Contacts OFF;
END

PRINT 'Database setup completed successfully!';
PRINT 'Tables created: Users, Categories, Budgets, SavingGoals, Transactions, Contacts, Transfers';
PRINT 'Indexes created for optimal performance';
PRINT 'Sample data inserted for testing';
GO
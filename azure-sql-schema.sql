-- FinancialApp Database Schema for Azure SQL
-- This will be executed on Azure SQL Database

-- Users table
CREATE TABLE [Users] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Email] NVARCHAR(255) UNIQUE NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    [PasswordHash] NVARCHAR(255) NOT NULL,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 DEFAULT GETUTCDATE()
);

-- Categories table
CREATE TABLE [Categories] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(MAX),
    [UserId] INT FOREIGN KEY REFERENCES [Users]([Id]),
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE()
);

-- Transactions table
CREATE TABLE [Transactions] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Amount] DECIMAL(18,2) NOT NULL,
    [Description] NVARCHAR(MAX),
    [Date] DATETIME2 NOT NULL,
    [Type] NVARCHAR(50) NOT NULL,
    [UserId] INT FOREIGN KEY REFERENCES [Users]([Id]),
    [CategoryId] INT FOREIGN KEY REFERENCES [Categories]([Id]),
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE()
);

-- Budgets table
CREATE TABLE [Budgets] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(255) NOT NULL,
    [Amount] DECIMAL(18,2) NOT NULL,
    [Period] NVARCHAR(50) NOT NULL,
    [UserId] INT FOREIGN KEY REFERENCES [Users]([Id]),
    [CategoryId] INT FOREIGN KEY REFERENCES [Categories]([Id]),
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE()
);

-- SavingGoals table
CREATE TABLE [SavingGoals] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(255) NOT NULL,
    [TargetAmount] DECIMAL(18,2) NOT NULL,
    [CurrentAmount] DECIMAL(18,2) DEFAULT 0,
    [TargetDate] DATETIME2,
    [UserId] INT FOREIGN KEY REFERENCES [Users]([Id]),
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE()
);

-- Sample Categories for testing
INSERT INTO [Categories] ([Name], [Description], [UserId]) VALUES
('Food & Dining', 'Restaurants, groceries, food delivery', NULL),
('Transportation', 'Gas, parking, public transport', NULL),
('Shopping', 'Clothing, electronics, personal items', NULL),
('Entertainment', 'Movies, games, hobbies', NULL),
('Bills & Utilities', 'Electricity, water, internet, phone', NULL),
('Healthcare', 'Medical expenses, pharmacy', NULL),
('Education', 'Courses, books, training', NULL),
('Travel', 'Flights, hotels, vacation expenses', NULL);

-- Sample test user (for testing purposes)
INSERT INTO [Users] ([Email], [Name], [PasswordHash]) VALUES
('test@example.com', 'Test User', '$2a$11$example.hashed.password.here');

-- Sample transactions for demo
INSERT INTO [Transactions] ([Amount], [Description], [Date], [Type], [UserId], [CategoryId]) VALUES
(-25.50, 'Lunch at restaurant', GETUTCDATE(), 'Expense', 1, 1),
(-60.00, 'Gas for car', GETUTCDATE(), 'Expense', 1, 2),
(1000.00, 'Salary payment', GETUTCDATE(), 'Income', 1, NULL),
(-15.99, 'Netflix subscription', GETUTCDATE(), 'Expense', 1, 4),
(-120.00, 'Electricity bill', GETUTCDATE(), 'Expense', 1, 5);
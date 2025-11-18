-- SQL Server Database Schema for FinancialApp
-- This will run on SQL Server container startup

-- Create database
CREATE DATABASE FinancialAppDB;
GO

USE FinancialAppDB;
GO

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

-- Sample Categories
INSERT INTO [Categories] ([Name], [Description], [UserId]) VALUES
('Food & Dining', 'Restaurants, groceries', NULL),
('Transportation', 'Gas, parking, transport', NULL),
('Shopping', 'Clothing, electronics', NULL),
('Entertainment', 'Movies, games', NULL),
('Bills & Utilities', 'Electricity, water, internet', NULL),
('Healthcare', 'Medical expenses', NULL),
('Education', 'Courses, books', NULL),
('Travel', 'Flights, hotels', NULL);
GO

PRINT 'Database initialized successfully!';
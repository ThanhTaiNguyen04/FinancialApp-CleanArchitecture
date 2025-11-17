-- PostgreSQL Schema for FinancialApp
-- Execute this in Railway PostgreSQL Data tab

-- Users table
CREATE TABLE "Users" (
    "Id" SERIAL PRIMARY KEY,
    "Email" VARCHAR(255) UNIQUE NOT NULL,
    "Name" VARCHAR(255) NOT NULL,
    "PasswordHash" VARCHAR(255) NOT NULL,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Categories table  
CREATE TABLE "Categories" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "Description" TEXT,
    "UserId" INTEGER REFERENCES "Users"("Id"),
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Transactions table
CREATE TABLE "Transactions" (
    "Id" SERIAL PRIMARY KEY,
    "Amount" DECIMAL(18,2) NOT NULL,
    "Description" TEXT,
    "Date" TIMESTAMP NOT NULL,
    "Type" VARCHAR(50) NOT NULL,
    "UserId" INTEGER REFERENCES "Users"("Id"),
    "CategoryId" INTEGER REFERENCES "Categories"("Id"),
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Budgets table
CREATE TABLE "Budgets" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "Amount" DECIMAL(18,2) NOT NULL,
    "Period" VARCHAR(50) NOT NULL,
    "UserId" INTEGER REFERENCES "Users"("Id"),
    "CategoryId" INTEGER REFERENCES "Categories"("Id"),
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- SavingGoals table
CREATE TABLE "SavingGoals" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "TargetAmount" DECIMAL(18,2) NOT NULL,
    "CurrentAmount" DECIMAL(18,2) DEFAULT 0,
    "TargetDate" TIMESTAMP,
    "UserId" INTEGER REFERENCES "Users"("Id"),
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Sample Categories
INSERT INTO "Categories" ("Name", "Description", "UserId") VALUES
('Food & Dining', 'Restaurants, groceries', NULL),
('Transportation', 'Gas, parking, transport', NULL),
('Shopping', 'Clothing, electronics', NULL),
('Entertainment', 'Movies, games', NULL),
('Bills & Utilities', 'Electricity, water, internet', NULL),
('Healthcare', 'Medical expenses', NULL),
('Education', 'Courses, books', NULL),
('Travel', 'Flights, hotels', NULL);
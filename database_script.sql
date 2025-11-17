IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Categories] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [IconName] nvarchar(50) NOT NULL,
    [ColorCode] nvarchar(20) NOT NULL,
    [Type] nvarchar(20) NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [FullName] nvarchar(100) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [PasswordHash] nvarchar(256) NOT NULL,
    [Phone] nvarchar(20) NOT NULL,
    [AvatarUrl] nvarchar(500) NOT NULL,
    [AvailableBalance] decimal(18,2) NOT NULL DEFAULT 0.0,
    [Role] nvarchar(50) NOT NULL,
    [IsPremium] bit NOT NULL,
    [PremiumExpiry] datetime2 NULL,
    [SubscriptionType] nvarchar(20) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Budgets] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [CategoryId] int NOT NULL,
    [BudgetAmount] decimal(18,2) NOT NULL,
    [SpentAmount] decimal(18,2) NOT NULL,
    [Month] int NOT NULL,
    [Year] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Budgets] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Budgets_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Budgets_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Contacts] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [FullName] nvarchar(100) NOT NULL,
    [Phone] nvarchar(20) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [AvatarUrl] nvarchar(500) NOT NULL,
    [IsRecent] bit NOT NULL,
    [LastTransactionDate] datetime2 NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Contacts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Contacts_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [PremiumRequests] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [RequestDate] datetime2 NOT NULL,
    [Status] nvarchar(450) NOT NULL DEFAULT N'Pending',
    [ApprovedBy] int NULL,
    [ApprovedDate] datetime2 NULL,
    [RejectionReason] nvarchar(max) NULL,
    [TransactionReference] nvarchar(max) NULL,
    [Amount] decimal(18,2) NOT NULL DEFAULT 29000.0,
    [Notes] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_PremiumRequests] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PremiumRequests_Users_ApprovedBy] FOREIGN KEY ([ApprovedBy]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PremiumRequests_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [SavingGoals] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    [TargetAmount] decimal(18,2) NOT NULL,
    [CurrentAmount] decimal(18,2) NOT NULL,
    [TargetDate] datetime2 NOT NULL,
    [IconName] nvarchar(50) NOT NULL,
    [ColorCode] nvarchar(20) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_SavingGoals] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SavingGoals_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Transactions] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [Type] nvarchar(20) NOT NULL,
    [Category] nvarchar(50) NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Description] nvarchar(200) NOT NULL,
    [IconName] nvarchar(50) NULL,
    [TransactionDate] datetime2 NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CategoryId] int NULL,
    CONSTRAINT [PK_Transactions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Transactions_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]),
    CONSTRAINT [FK_Transactions_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Transfers] (
    [Id] int NOT NULL IDENTITY,
    [FromUserId] int NOT NULL,
    [ToContactId] int NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [TransferType] nvarchar(20) NOT NULL,
    [Description] nvarchar(200) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [TransferDate] datetime2 NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Transfers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Transfers_Contacts_ToContactId] FOREIGN KEY ([ToContactId]) REFERENCES [Contacts] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Transfers_Users_FromUserId] FOREIGN KEY ([FromUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ColorCode', N'CreatedAt', N'IconName', N'IsActive', N'Name', N'Type') AND [object_id] = OBJECT_ID(N'[Categories]'))
    SET IDENTITY_INSERT [Categories] ON;
INSERT INTO [Categories] ([Id], [ColorCode], [CreatedAt], [IconName], [IsActive], [Name], [Type])
VALUES (1, N'#4CAF50', '2025-09-26T10:00:00.0000000Z', N'salary', CAST(1 AS bit), N'Lương', N'income'),
(2, N'#FF9800', '2025-09-26T10:00:00.0000000Z', N'shopping', CAST(1 AS bit), N'Đi chợ', N'expense'),
(3, N'#2196F3', '2025-09-26T10:00:00.0000000Z', N'home', CAST(1 AS bit), N'Thuê nhà', N'expense'),
(4, N'#F44336', '2025-09-26T10:00:00.0000000Z', N'medical', CAST(1 AS bit), N'Y tế', N'expense'),
(5, N'#9C27B0', '2025-09-26T10:00:00.0000000Z', N'travel', CAST(1 AS bit), N'Du lịch', N'expense');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ColorCode', N'CreatedAt', N'IconName', N'IsActive', N'Name', N'Type') AND [object_id] = OBJECT_ID(N'[Categories]'))
    SET IDENTITY_INSERT [Categories] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AvailableBalance', N'AvatarUrl', N'CreatedAt', N'Email', N'FullName', N'IsPremium', N'PasswordHash', N'Phone', N'PremiumExpiry', N'Role', N'SubscriptionType', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] ON;
INSERT INTO [Users] ([Id], [AvailableBalance], [AvatarUrl], [CreatedAt], [Email], [FullName], [IsPremium], [PasswordHash], [Phone], [PremiumExpiry], [Role], [SubscriptionType], [UpdatedAt])
VALUES (1, 5320.5, N'https://images.pexels.com/photos/2167673/pexels-photo-2167673.jpeg?auto=compress&cs=tinysrgb&dpr=3&h=750&w=1260', '2025-09-26T10:00:00.0000000Z', N'christopher.summers@email.com', N'Christopher Summers', CAST(0 AS bit), N'', N'+1234567890', NULL, N'User', N'Free', '2025-09-26T10:00:00.0000000Z');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AvailableBalance', N'AvatarUrl', N'CreatedAt', N'Email', N'FullName', N'IsPremium', N'PasswordHash', N'Phone', N'PremiumExpiry', N'Role', N'SubscriptionType', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'BudgetAmount', N'CategoryId', N'CreatedAt', N'Month', N'SpentAmount', N'UpdatedAt', N'UserId', N'Year') AND [object_id] = OBJECT_ID(N'[Budgets]'))
    SET IDENTITY_INSERT [Budgets] ON;
INSERT INTO [Budgets] ([Id], [BudgetAmount], [CategoryId], [CreatedAt], [Month], [SpentAmount], [UpdatedAt], [UserId], [Year])
VALUES (1, 2000000.0, 2, '2025-09-26T10:00:00.0000000Z', 9, 455789.0, '2025-09-26T10:00:00.0000000Z', 1, 2025),
(2, 3500000.0, 3, '2025-09-26T10:00:00.0000000Z', 9, 3000000.0, '2025-09-26T10:00:00.0000000Z', 1, 2025),
(3, 1000000.0, 4, '2025-09-26T10:00:00.0000000Z', 9, 243789.0, '2025-09-26T10:00:00.0000000Z', 1, 2025);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'BudgetAmount', N'CategoryId', N'CreatedAt', N'Month', N'SpentAmount', N'UpdatedAt', N'UserId', N'Year') AND [object_id] = OBJECT_ID(N'[Budgets]'))
    SET IDENTITY_INSERT [Budgets] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AvatarUrl', N'CreatedAt', N'Email', N'FullName', N'IsRecent', N'LastTransactionDate', N'Phone', N'UserId') AND [object_id] = OBJECT_ID(N'[Contacts]'))
    SET IDENTITY_INSERT [Contacts] ON;
INSERT INTO [Contacts] ([Id], [AvatarUrl], [CreatedAt], [Email], [FullName], [IsRecent], [LastTransactionDate], [Phone], [UserId])
VALUES (1, N'https://images.pexels.com/photos/733872/pexels-photo-733872.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=750&w=1260', '2025-09-26T10:00:00.0000000Z', N'catherine@email.com', N'Catherine Johnson', CAST(1 AS bit), NULL, N'+1234567891', 1),
(2, N'https://images.pexels.com/photos/697509/pexels-photo-697509.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=750&w=1260', '2025-09-26T10:00:00.0000000Z', N'allan@email.com', N'Allan Smith', CAST(1 AS bit), NULL, N'+1234567892', 1),
(3, N'https://images.pexels.com/photos/2748091/pexels-photo-2748091.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=750&w=1260', '2025-09-26T10:00:00.0000000Z', N'kimberly@email.com', N'Kimberly Brown', CAST(1 AS bit), NULL, N'+1234567893', 1);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AvatarUrl', N'CreatedAt', N'Email', N'FullName', N'IsRecent', N'LastTransactionDate', N'Phone', N'UserId') AND [object_id] = OBJECT_ID(N'[Contacts]'))
    SET IDENTITY_INSERT [Contacts] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ColorCode', N'CreatedAt', N'CurrentAmount', N'Description', N'IconName', N'Name', N'Status', N'TargetAmount', N'TargetDate', N'UpdatedAt', N'UserId') AND [object_id] = OBJECT_ID(N'[SavingGoals]'))
    SET IDENTITY_INSERT [SavingGoals] ON;
INSERT INTO [SavingGoals] ([Id], [ColorCode], [CreatedAt], [CurrentAmount], [Description], [IconName], [Name], [Status], [TargetAmount], [TargetDate], [UpdatedAt], [UserId])
VALUES (1, N'#4CAF50', '2025-09-26T10:00:00.0000000Z', 2300000.0, N'Tiết kiệm cho việc học', N'education', N'Giáo dục', N'active', 5000000.0, '2026-03-26T10:00:00.0000000Z', '2025-09-26T10:00:00.0000000Z', 1),
(2, N'#FF9800', '2025-09-26T10:00:00.0000000Z', 5600000.0, N'Tiết kiệm mua nhà', N'house', N'Mua nhà', N'active', 47000000.0, '2027-09-26T10:00:00.0000000Z', '2025-09-26T10:00:00.0000000Z', 1),
(3, N'#2196F3', '2025-09-26T10:00:00.0000000Z', 800000.0, N'Chuyến du lịch Hàn Quốc', N'travel', N'Du lịch', N'active', 2200000.0, '2026-01-26T10:00:00.0000000Z', '2025-09-26T10:00:00.0000000Z', 1);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ColorCode', N'CreatedAt', N'CurrentAmount', N'Description', N'IconName', N'Name', N'Status', N'TargetAmount', N'TargetDate', N'UpdatedAt', N'UserId') AND [object_id] = OBJECT_ID(N'[SavingGoals]'))
    SET IDENTITY_INSERT [SavingGoals] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Amount', N'Category', N'CategoryId', N'CreatedAt', N'Description', N'IconName', N'TransactionDate', N'Type', N'UserId') AND [object_id] = OBJECT_ID(N'[Transactions]'))
    SET IDENTITY_INSERT [Transactions] ON;
INSERT INTO [Transactions] ([Id], [Amount], [Category], [CategoryId], [CreatedAt], [Description], [IconName], [TransactionDate], [Type], [UserId])
VALUES (1, 16000000.0, N'Lương', NULL, '2025-09-26T10:00:00.0000000Z', N'Lương tháng 9', NULL, '2025-09-21T10:00:00.0000000Z', N'income', 1),
(2, 455789.0, N'Đi chợ', NULL, '2025-09-26T10:00:00.0000000Z', N'Mua sắm hàng tuần', NULL, '2025-09-23T10:00:00.0000000Z', N'expense', 1),
(3, 3000000.0, N'Thuê nhà', NULL, '2025-09-26T10:00:00.0000000Z', N'Tiền thuê nhà tháng 9', NULL, '2025-09-24T10:00:00.0000000Z', N'expense', 1),
(4, 243789.0, N'Y tế', NULL, '2025-09-26T10:00:00.0000000Z', N'Khám bệnh và thuốc', NULL, '2025-09-25T10:00:00.0000000Z', N'expense', 1);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Amount', N'Category', N'CategoryId', N'CreatedAt', N'Description', N'IconName', N'TransactionDate', N'Type', N'UserId') AND [object_id] = OBJECT_ID(N'[Transactions]'))
    SET IDENTITY_INSERT [Transactions] OFF;
GO

CREATE INDEX [IX_Budgets_CategoryId] ON [Budgets] ([CategoryId]);
GO

CREATE INDEX [IX_Budgets_UserId] ON [Budgets] ([UserId]);
GO

CREATE UNIQUE INDEX [IX_Contacts_UserId_Phone] ON [Contacts] ([UserId], [Phone]);
GO

CREATE INDEX [IX_PremiumRequests_ApprovedBy] ON [PremiumRequests] ([ApprovedBy]);
GO

CREATE INDEX [IX_PremiumRequests_Status] ON [PremiumRequests] ([Status]);
GO

CREATE INDEX [IX_PremiumRequests_UserId_RequestDate] ON [PremiumRequests] ([UserId], [RequestDate]);
GO

CREATE INDEX [IX_SavingGoals_UserId] ON [SavingGoals] ([UserId]);
GO

CREATE INDEX [IX_Transactions_CategoryId] ON [Transactions] ([CategoryId]);
GO

CREATE INDEX [IX_Transactions_UserId_TransactionDate] ON [Transactions] ([UserId], [TransactionDate]);
GO

CREATE INDEX [IX_Transfers_FromUserId_TransferDate] ON [Transfers] ([FromUserId], [TransferDate]);
GO

CREATE INDEX [IX_Transfers_ToContactId] ON [Transfers] ([ToContactId]);
GO

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251023174246_InitialCreate', N'8.0.10');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [ChatMessages] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [Message] nvarchar(max) NOT NULL,
    [Response] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [MessageType] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_ChatMessages] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ChatMessages_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_ChatMessages_UserId] ON [ChatMessages] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251027181016_AddChatMessageTable', N'8.0.10');
GO

COMMIT;
GO


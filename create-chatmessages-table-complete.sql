-- Script để tạo bảng ChatMessages
-- Chạy script này trong SQL Server Management Studio hoặc Azure Data Studio

USE [FinancialAppDB]; -- Thay đổi tên database nếu khác
GO

-- Kiểm tra và xóa bảng cũ nếu tồn tại (tùy chọn - bỏ comment nếu muốn reset)
-- IF OBJECT_ID('dbo.ChatMessages', 'U') IS NOT NULL
-- BEGIN
--     DROP TABLE dbo.ChatMessages;
--     PRINT 'Đã xóa bảng ChatMessages cũ';
-- END
-- GO

-- Tạo bảng ChatMessages mới
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ChatMessages' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[ChatMessages] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [UserId] int NOT NULL,
        [Message] nvarchar(max) NOT NULL,
        [Response] nvarchar(max) NULL,
        [CreatedAt] datetime2(7) NOT NULL DEFAULT GETDATE(),
        [IsFromUser] bit NOT NULL DEFAULT 1,
        [MessageType] nvarchar(50) NOT NULL DEFAULT 'text',
        CONSTRAINT [PK_ChatMessages] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    -- Tạo index để tăng hiệu suất query
    CREATE NONCLUSTERED INDEX [IX_ChatMessages_UserId] ON [dbo].[ChatMessages] ([UserId]);
    CREATE NONCLUSTERED INDEX [IX_ChatMessages_CreatedAt] ON [dbo].[ChatMessages] ([CreatedAt]);

    PRINT 'Đã tạo bảng ChatMessages thành công!';
END
ELSE
BEGIN
    PRINT 'Bảng ChatMessages đã tồn tại.';
END
GO

-- Kiểm tra bảng đã được tạo
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'ChatMessages'
ORDER BY ORDINAL_POSITION;

-- Thêm dữ liệu mẫu (tùy chọn)
IF NOT EXISTS (SELECT * FROM ChatMessages)
BEGIN
    INSERT INTO ChatMessages (UserId, Message, Response, IsFromUser, MessageType, CreatedAt)
    VALUES 
        (1, 'Xin chào!', 'Chào bạn! Tôi có thể giúp gì cho bạn về tài chính?', 1, 'text', GETDATE()),
        (1, '', 'Chào bạn! Tôi có thể giúp gì cho bạn về tài chính?', 0, 'text', GETDATE());
    
    PRINT 'Đã thêm dữ liệu mẫu vào bảng ChatMessages.';
END

-- Hiển thị số lượng record trong bảng
SELECT COUNT(*) as 'Tổng số tin nhắn' FROM ChatMessages;

PRINT 'Script hoàn thành!';
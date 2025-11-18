-- Script để kiểm tra và sửa bảng ChatMessages hoàn chỉnh
-- Chạy script này để đảm bảo chatbot hoạt động

USE [FinancialAppDB]; -- Thay đổi tên database nếu khác
GO

-- 1. Kiểm tra bảng có tồn tại không
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ChatMessages' AND xtype='U')
BEGIN
    PRINT '❌ Bảng ChatMessages chưa tồn tại. Đang tạo...';
    
    CREATE TABLE [dbo].[ChatMessages] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [UserId] int NOT NULL,
        [Message] nvarchar(max) NOT NULL,
        [Response] nvarchar(max) NOT NULL DEFAULT '',
        [CreatedAt] datetime2(7) NOT NULL DEFAULT GETDATE(),
        [MessageType] nvarchar(50) NOT NULL DEFAULT 'text',
        CONSTRAINT [PK_ChatMessages] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    PRINT '✅ Đã tạo bảng ChatMessages';
END
ELSE
BEGIN
    PRINT '✅ Bảng ChatMessages đã tồn tại';
END
GO

-- 2. Kiểm tra và thêm cột MessageType nếu thiếu
IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'ChatMessages' AND COLUMN_NAME = 'MessageType'
)
BEGIN
    ALTER TABLE [dbo].[ChatMessages]
    ADD [MessageType] nvarchar(50) NOT NULL DEFAULT 'text';
    
    PRINT '✅ Đã thêm cột MessageType';
END
ELSE
BEGIN
    PRINT '✅ Cột MessageType đã tồn tại';
END
GO

-- 3. Cập nhật dữ liệu null/empty
UPDATE ChatMessages 
SET MessageType = 'text' 
WHERE MessageType IS NULL OR MessageType = '';

UPDATE ChatMessages 
SET Response = '' 
WHERE Response IS NULL;

-- 4. Tạo index để tăng performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ChatMessages_UserId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ChatMessages_UserId] ON [dbo].[ChatMessages] ([UserId]);
    PRINT '✅ Đã tạo index IX_ChatMessages_UserId';
END

-- 5. Thêm dữ liệu mẫu cho test
IF NOT EXISTS (SELECT * FROM ChatMessages WHERE UserId = 1)
BEGIN
    INSERT INTO ChatMessages (UserId, Message, Response, MessageType)
    VALUES 
        (1, 'Xin chào!', '🤖 Chào bạn! Tôi là AI Financial Assistant của FinancialApp. Tôi có thể giúp gì cho bạn về tài chính?', 'text'),
        (1, 'Test chatbot', '✅ Chatbot đang hoạt động bình thường! Bạn có thể hỏi tôi về quản lý tài chính.', 'test');
    
    PRINT '✅ Đã thêm dữ liệu mẫu';
END

-- 6. Hiển thị thông tin bảng
PRINT '📋 THÔNG TIN BẢNG CHATMESSAGES:';
SELECT 
    COLUMN_NAME as 'Tên cột',
    DATA_TYPE as 'Kiểu dữ liệu', 
    IS_NULLABLE as 'Cho phép NULL',
    COLUMN_DEFAULT as 'Giá trị mặc định'
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'ChatMessages'
ORDER BY ORDINAL_POSITION;

-- 7. Hiển thị dữ liệu mẫu
PRINT '💬 DỮ LIỆU MẪU:';
SELECT TOP 3 
    Id, UserId, 
    LEFT(Message, 50) as 'Message (50 ký tự đầu)',
    LEFT(Response, 50) as 'Response (50 ký tự đầu)',
    MessageType, CreatedAt
FROM ChatMessages 
ORDER BY CreatedAt DESC;

PRINT '🎉 HOÀN THÀNH! Bảng ChatMessages đã sẵn sàng!';
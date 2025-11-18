-- Script đơn giản để tạo bảng ChatMessages
-- Sử dụng script này nếu script phức tạp gặp lỗi

CREATE TABLE [ChatMessages] (
    [Id] int IDENTITY(1,1) PRIMARY KEY,
    [UserId] int NOT NULL,
    [Message] nvarchar(max) NOT NULL,
    [Response] nvarchar(max) NULL,
    [CreatedAt] datetime2(7) NOT NULL DEFAULT GETDATE(),
    [IsFromUser] bit NOT NULL DEFAULT 1
);

-- Thêm tin nhắn mẫu
INSERT INTO ChatMessages (UserId, Message, Response, IsFromUser)
VALUES (1, 'Xin chào!', 'Chào bạn! Tôi có thể giúp gì cho bạn?', 1);
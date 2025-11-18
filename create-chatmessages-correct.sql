-- Script hoàn chỉnh để tạo bảng ChatMessages với đầy đủ các cột
-- Chạy script này nếu muốn tạo lại bảng từ đầu

CREATE TABLE [ChatMessages] (
    [Id] int IDENTITY(1,1) PRIMARY KEY,
    [UserId] int NOT NULL,
    [Message] nvarchar(max) NOT NULL,
    [Response] nvarchar(max) NOT NULL DEFAULT '',
    [CreatedAt] datetime2(7) NOT NULL DEFAULT GETDATE(),
    [MessageType] nvarchar(50) NOT NULL DEFAULT 'text'
);

-- Thêm dữ liệu mẫu
INSERT INTO ChatMessages (UserId, Message, Response, MessageType)
VALUES 
    (1, 'Xin chào!', 'Chào bạn! Tôi có thể giúp gì cho bạn về tài chính?', 'text'),
    (1, 'Tôi muốn xem báo cáo chi tiêu', 'Tôi sẽ tạo báo cáo chi tiêu cho bạn ngay.', 'summary');

-- Kiểm tra
SELECT * FROM ChatMessages;
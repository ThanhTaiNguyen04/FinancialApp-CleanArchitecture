-- SCRIPT KHẨN CẤP - KÍCH HOẠT AI CHATBOT
-- Chạy script này ngay để kích hoạt AI

-- 1. Thêm cột MessageType nếu chưa có
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ChatMessages' AND COLUMN_NAME = 'MessageType')
BEGIN
    ALTER TABLE ChatMessages ADD MessageType nvarchar(50) NOT NULL DEFAULT 'text';
    PRINT '✅ Đã thêm cột MessageType';
END
ELSE
BEGIN
    PRINT '✅ Cột MessageType đã tồn tại';
END

-- 2. Cập nhật dữ liệu cũ
UPDATE ChatMessages SET MessageType = 'text' WHERE MessageType IS NULL OR MessageType = '';

-- 3. Kiểm tra kết quả
SELECT COUNT(*) as 'Tổng tin nhắn', MessageType FROM ChatMessages GROUP BY MessageType;

PRINT '🚀 HOÀN THÀNH! AI đã được kích hoạt!';
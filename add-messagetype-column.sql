-- Script để thêm cột MessageType vào bảng ChatMessages
-- Chạy script này để sửa lỗi "Invalid column name 'MessageType'"

USE [FinancialAppDB]; -- Thay đổi tên database nếu khác
GO

-- Kiểm tra xem cột MessageType đã tồn tại chưa
IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'ChatMessages' AND COLUMN_NAME = 'MessageType'
)
BEGIN
    -- Thêm cột MessageType
    ALTER TABLE [dbo].[ChatMessages]
    ADD [MessageType] nvarchar(50) NOT NULL DEFAULT 'text';
    
    PRINT 'Đã thêm cột MessageType vào bảng ChatMessages.';
END
ELSE
BEGIN
    PRINT 'Cột MessageType đã tồn tại trong bảng ChatMessages.';
END
GO

-- Cập nhật dữ liệu hiện có để có MessageType
UPDATE ChatMessages 
SET MessageType = 'text' 
WHERE MessageType IS NULL OR MessageType = '';

-- Kiểm tra cấu trúc bảng sau khi cập nhật
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'ChatMessages'
ORDER BY ORDINAL_POSITION;

-- Hiển thị dữ liệu mẫu
SELECT TOP 5 * FROM ChatMessages;

PRINT 'Script hoàn thành! Cột MessageType đã được thêm.';
# HƯỚNG DẪN SỬA LỖI CHATBOT - TÍNH NĂNG CHAT

## VẤN ĐỀ
Lỗi: `Invalid object name 'ChatMessages'` - Bảng ChatMessages chưa tồn tại trong database.

## GIẢI PHÁP

### Bước 1: Chạy Script SQL
Bạn có 2 tùy chọn:

#### Tùy chọn A: Script đầy đủ (khuyến nghị)
```sql
-- Chạy file: create-chatmessages-table-complete.sql
```

#### Tùy chọn B: Script đơn giản
```sql
-- Chạy file: create-chatmessages-simple.sql
```

### Bước 2: Cách thực hiện

#### Nếu dùng SQL Server Management Studio (SSMS):
1. Mở SSMS
2. Kết nối tới database của bạn
3. Mở file `create-chatmessages-table-complete.sql`
4. Sửa tên database trong dòng `USE [FinancialAppDB];` thành tên database thực tế của bạn
5. Nhấn F5 để chạy script

#### Nếu dùng Azure Data Studio:
1. Mở Azure Data Studio
2. Kết nối tới database
3. Mở file SQL
4. Chạy script

#### Nếu dùng Visual Studio:
1. Mở Server Explorer
2. Kết nối tới database
3. Right-click → New Query
4. Copy nội dung từ file SQL và paste vào
5. Execute

### Bước 3: Kiểm tra
Sau khi chạy script, kiểm tra bảng đã được tạo:
```sql
SELECT * FROM ChatMessages;
```

### Bước 4: Khởi động lại Backend
Sau khi tạo bảng, khởi động lại backend:
```
dotnet run
```

## CẤU TRÚC BẢNG CHATMESSAGES

| Cột | Kiểu dữ liệu | Mô tả |
|-----|-------------|--------|
| Id | int IDENTITY | Khóa chính, tự tăng |
| UserId | int | ID của người dùng |
| Message | nvarchar(max) | Tin nhắn từ người dùng |
| Response | nvarchar(max) | Phản hồi từ chatbot |
| CreatedAt | datetime2 | Thời gian tạo |
| IsFromUser | bit | True nếu từ user, False nếu từ bot |

## TÍNH NĂNG CHATBOT

Sau khi sửa lỗi, chatbot sẽ có các tính năng:
- Lưu trữ lịch sử chat
- Phản hồi tự động về tài chính
- Giao diện chat thân thiện
- Hỗ trợ nhiều người dùng

## LỖI THƯỜNG GẶP

1. **"Database not found"**: Kiểm tra connection string
2. **"Permission denied"**: Đảm bảo có quyền tạo bảng
3. **"Syntax error"**: Kiểm tra SQL syntax phù hợp với SQL Server

## LIÊN HỆ
Nếu vẫn gặp lỗi, vui lòng cung cấp:
- Loại database đang sử dụng
- Connection string (bỏ password)
- Error message chi tiết
# HƯỚNG DẪN SỬA LỖI CHATBOT HOÀN CHỈNH

## 🎯 MỤC TIÊU
Sửa lỗi 500 khi gửi tin nhắn chatbot và làm cho tính năng chat hoạt động bình thường.

## 📋 CÁC BƯỚC THỰC HIỆN

### BƯỚC 1: Chạy Script Sửa Database ⚠️ QUAN TRỌNG
1. Mở **SQL Server Management Studio** (SSMS)
2. Kết nối tới database của bạn
3. Mở file: `fix-chatmessages-complete.sql`
4. **Sửa dòng 4**: Thay `[FinancialAppDB]` thành tên database thực tế của bạn
5. Nhấn **F5** để chạy script
6. Đảm bảo thấy thông báo "🎉 HOÀN THÀNH! Bảng ChatMessages đã sẵn sàng!"

### BƯỚC 2: Khởi Động Lại Backend
```bash
# Trong terminal tại D:\EXE201\FinancialApp-CleanArchitecture
dotnet run
```

### BƯỚC 3: Test API Endpoints
Trước khi test trên mobile, hãy test các endpoint:

#### 3.1 Test API hoạt động:
```
GET http://localhost:5000/api/Chat/test
```
Phải trả về: `{"message": "Chat API is working!", "timestamp": "..."}`

#### 3.2 Test endpoint gửi tin nhắn đơn giản:
```
POST http://localhost:5000/api/Chat/test-message
Content-Type: application/json
Authorization: Bearer YOUR_JWT_TOKEN

{
  "message": "Hello test"
}
```

### BƯỚC 4: Test Trên Mobile App
1. Khởi động mobile app
2. Đăng nhập vào tài khoản
3. Vào màn hình Chat
4. Gửi tin nhắn: **"test"** (để dùng endpoint test)
5. Gửi tin nhắn khác để test endpoint chính

## 🔧 CÁC ENDPOINT HIỆN TẠI

### Đã Sửa/Thêm:
- `GET /api/Chat/test` - Test API hoạt động
- `POST /api/Chat/test-message` - Test gửi tin nhắn đơn giản 
- `POST /api/Chat/message` - Endpoint chính (đã sửa để không gọi AI)
- `GET /api/Chat/history` - Lấy lịch sử chat
- `GET /api/Chat/setup` - Tự động tạo bảng

### Logic Đã Thay Đổi:
- ✅ Loại bỏ AI API call tạm thời
- ✅ Trả về response đơn giản
- ✅ Lưu tin nhắn vào database
- ✅ Xử lý lỗi tốt hơn

## 🐛 XỬ LÝ LỖI

### Nếu vẫn lỗi 500:
1. Kiểm tra backend console log
2. Kiểm tra database connection string
3. Đảm bảo bảng ChatMessages đã được tạo đúng

### Nếu lỗi JWT/Authentication:
1. Đảm bảo đã đăng nhập
2. Token JWT phải hợp lệ
3. Kiểm tra header Authorization

### Nếu lỗi database:
1. Chạy lại script `fix-chatmessages-complete.sql`
2. Kiểm tra kết nối database
3. Đảm bảo user có quyền tạo/sửa bảng

## ✅ KẾT QUẢ MONG ĐỢI

Sau khi hoàn thành:
- ✅ Chatbot nhận và phản hồi tin nhắn
- ✅ Lưu lịch sử chat vào database  
- ✅ Hiển thị tin nhắn trên mobile app
- ✅ Không còn lỗi 500

## 🚀 TÍNH NĂNG TƯƠNG LAI

Sau khi sửa xong lỗi cơ bản, có thể:
- Kích hoạt lại AI API (Groq)
- Thêm tính năng phân tích tài chính
- Cải thiện UI/UX chat
- Thêm voice chat

## 📞 HỖ TRỢ

Nếu vẫn gặp lỗi, vui lòng cung cấp:
1. Log từ backend console
2. Log từ mobile app console  
3. Screenshot lỗi
4. Tên database đang sử dụng
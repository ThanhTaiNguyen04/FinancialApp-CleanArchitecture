using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinancialApp.Infrastructure.Data;
using FinancialApp.Application.DTOs;
using FinancialApp.Domain.Entities;
using System.Text.Json;
using System.Text;
using System.Security.Claims;

namespace FinancialApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ChatController> _logger;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string _groqApiKey;

    public ChatController(ApplicationDbContext context, ILogger<ChatController> logger, HttpClient httpClient, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _httpClient = httpClient;
        _configuration = configuration;
        _groqApiKey = _configuration["Groq:ApiKey"] ?? throw new InvalidOperationException("Groq API Key not configured");
    }

    // POST: api/Chat/message
    // Send message to AI Financial Assistant (Premium only)
    [HttpPost("message")]
    public async Task<ActionResult<ChatResponseDto>> SendMessage([FromBody] SendChatMessageDto request)
    {
        int userId = 0;
        try
        {
            // Get userId from JWT token
            userId = GetCurrentUserId();

            // Check if user exists and has Premium
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Check Premium status - temporarily disabled for testing
            /*
            bool isPremium = user.SubscriptionType == "Premium" && 
                           user.PremiumExpiry.HasValue && 
                           user.PremiumExpiry.Value > DateTime.UtcNow;

            if (!isPremium)
            {
                return Forbid("This feature is only available for Premium users");
            }
            */

            // Process the message
            var response = await ProcessUserMessage(userId, request.Message);

            // Save chat history
            var chatMessage = new ChatMessage
            {
                UserId = userId,
                Message = request.Message,
                Response = response.Response,
                MessageType = response.MessageType,
                CreatedAt = DateTime.UtcNow
            };

            _context.ChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync();

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat message for user {UserId}", userId);
            return StatusCode(500, new { message = "An error occurred while processing your message" });
        }
    }

    // GET: api/Chat/test
    // Simple test endpoint
    [HttpGet("test")]
    public ActionResult<object> Test()
    {
        return Ok(new { message = "Chat API is working!", timestamp = DateTime.UtcNow });
    }

    // POST: api/Chat/test-message
    // Simple test message endpoint
    [HttpPost("test-message")]
    public async Task<ActionResult<object>> TestMessage([FromBody] SendChatMessageDto request)
    {
        try
        {
            _logger.LogInformation("🧪 Testing message: {Message}", request.Message);
            
            // Get userId from JWT token
            var userId = GetCurrentUserId();
            _logger.LogInformation("🆔 User ID: {UserId}", userId);

            // Simple response without AI
            var simpleResponse = $"Xin chào! Bạn đã gửi: '{request.Message}'. Đây là phản hồi test.";

            // Save to database
            var chatMessage = new ChatMessage
            {
                UserId = userId,
                Message = request.Message,
                Response = simpleResponse,
                MessageType = "test",
                CreatedAt = DateTime.UtcNow
            };

            _context.ChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync();

            return Ok(new { 
                response = simpleResponse, 
                messageType = "test",
                timestamp = DateTime.UtcNow,
                saved = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error in test message");
            return StatusCode(500, new { message = "Test message failed", error = ex.Message });
        }
    }

    // GET: api/Chat/history
    // Get chat history for Premium user
    [HttpGet("history")]
    public async Task<ActionResult<List<ChatMessageDto>>> GetChatHistory()
    {
        int userId = 0;
        try
        {
            _logger.LogInformation("🔍 Getting chat history...");
            
            // Get userId from JWT token
            userId = GetCurrentUserId();
            _logger.LogInformation("🆔 User ID from token: {UserId}", userId);

            var user = await _context.Users.FindAsync(userId);
            _logger.LogInformation("👤 User found: {UserEmail}, Premium: {SubscriptionType}", 
                user?.Email, user?.SubscriptionType);
            
            if (user == null)
            {
                _logger.LogWarning("❌ User not found for ID: {UserId}", userId);
                return NotFound(new { message = "User not found" });
            }

            // Check Premium status - temporarily disabled for testing
            /*
            bool isPremium = user.SubscriptionType == "Premium" && 
                           user.PremiumExpiry.HasValue && 
                           user.PremiumExpiry.Value > DateTime.UtcNow;

            if (!isPremium)
            {
                return Forbid("This feature is only available for Premium users");
            }
            */

            var chatHistory = await _context.ChatMessages
                .Where(cm => cm.UserId == userId)
                .OrderByDescending(cm => cm.CreatedAt)
                .Take(50) // Last 50 messages
                .Select(cm => new ChatMessageDto
                {
                    Id = cm.Id,
                    UserId = cm.UserId,
                    Message = cm.Message,
                    Response = cm.Response,
                    CreatedAt = cm.CreatedAt,
                    MessageType = cm.MessageType
                })
                .ToListAsync();

            _logger.LogInformation("💬 Found {Count} chat messages for user {UserId}", chatHistory.Count, userId);

            return Ok(chatHistory.OrderBy(cm => cm.CreatedAt)); // Return in chronological order
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting chat history for user {UserId}. Exception: {Exception}", userId, ex.ToString());
            return StatusCode(500, new { message = "An error occurred while retrieving chat history", error = ex.Message });
        }
    }

    // DELETE: api/Chat/history
    // Clear all chat history for current user
    [HttpDelete("history")]
    public async Task<ActionResult> ClearChatHistory()
    {
        int userId = 0;
        try
        {
            _logger.LogInformation("🗑️ Clearing chat history...");
            
            // Get userId from JWT token
            userId = GetCurrentUserId();
            _logger.LogInformation("🆔 User ID from token: {UserId}", userId);

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("❌ User not found for ID: {UserId}", userId);
                return NotFound(new { message = "User not found" });
            }

            // Get all chat messages for this user
            var chatMessages = await _context.ChatMessages
                .Where(cm => cm.UserId == userId)
                .ToListAsync();

            if (chatMessages.Any())
            {
                _context.ChatMessages.RemoveRange(chatMessages);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("✅ Cleared {Count} chat messages for user {UserId}", chatMessages.Count, userId);
                return Ok(new { message = $"Đã xóa {chatMessages.Count} tin nhắn", deletedCount = chatMessages.Count });
            }
            else
            {
                _logger.LogInformation("ℹ️ No chat messages to delete for user {UserId}", userId);
                return Ok(new { message = "Không có lịch sử chat để xóa", deletedCount = 0 });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing chat history for user {UserId}. Exception: {Exception}", userId, ex.ToString());
            return StatusCode(500, new { message = "Lỗi khi xóa lịch sử chat", error = ex.Message });
        }
    }

    private async Task<ChatResponseDto> ProcessUserMessage(int userId, string message)
    {
        try
        {
            // Check for special commands first
            if (message.StartsWith("/"))
            {
                return await ProcessSpecialCommand(userId, message);
            }

            // Smart natural language processing - detect intent
            var response = await ProcessNaturalLanguage(userId, message);
            if (response != null)
            {
                return response;
            }

            // Create simple system prompt without complex financial data (to avoid errors)
            var systemPrompt = $@"Bạn là chatbot quản lý chi tiêu thông minh.
Hãy giúp người dùng ghi chép chi tiêu, tổng hợp và gợi ý tiết kiệm hợp lý.
Nếu họ nói 'ghi chi tiêu', hãy hỏi họ thêm chi tiết (loại, số tiền, ghi chú).

Hãy tư vấn tài chính cho người dùng. Trả lời bằng tiếng Việt, thân thiện và chuyên nghiệp.
Sử dụng emoji phù hợp và đưa ra lời khuyên cụ thể, thực tế về:
- Cách quản lý tiền bạc hiệu quả  
- Mẹo tiết kiệm chi tiêu
- Lập kế hoạch tài chính
- Đầu tư an toàn

Giữ câu trả lời ngắn gọn và hữu ích (khoảng 100-150 từ).

Câu hỏi của người dùng: {message}";

            // Call Groq AI API
            var aiResponse = await CallGroqAPI(systemPrompt, message);

            return new ChatResponseDto
            {
                Response = aiResponse,
                MessageType = "ai_advice",
                Data = null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ProcessUserMessage: {Error}", ex.Message);
            
            // Try simple AI call without complex data
            try
            {
                var simplePrompt = "Bạn là chatbot quản lý chi tiêu thông minh. Hãy giúp người dùng ghi chép chi tiêu, tổng hợp và gợi ý tiết kiệm hợp lý. Trả lời câu hỏi này bằng tiếng Việt một cách ngắn gọn và hữu ích: " + message;
                var simpleAiResponse = await CallGroqAPI(simplePrompt, "");
                
                return new ChatResponseDto
                {
                    Response = simpleAiResponse,
                    MessageType = "simple_ai",
                    Data = null
                };
            }
            catch
            {
                // Final fallback
                return new ChatResponseDto
                {
                    Response = "💰 Xin chào! Tôi là AI Financial Assistant. Tôi có thể giúp bạn:\n\n📊 Lập kế hoạch tài chính\n💡 Tư vấn tiết kiệm\n📈 Quản lý chi tiêu\n\nHãy hỏi tôi bất cứ điều gì về tài chính! Ví dụ: 'Làm sao để tiết kiệm hiệu quả?'",
                    MessageType = "fallback",
                    Data = null
                };
            }
        }
    }

    private async Task<ChatResponseDto> ProcessSpecialCommand(int userId, string command)
    {
        var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var cmd = parts[0].ToLower();

        switch (cmd)
        {
            case "/summary":
                var summary = await GetUserFinancialContext(userId);
                return new ChatResponseDto
                {
                    Response = $@"📊 **Tóm tắt tài chính của bạn:**

💰 Thu nhập tháng này: {summary.TotalIncome:N0} VND
💸 Chi tiêu tháng này: {summary.TotalExpenses:N0} VND
💵 Số dư hiện tại: {summary.Balance:N0} VND

📈 **Top 3 danh mục chi tiêu:**
{string.Join("\n", summary.CategoryBreakdown.OrderByDescending(x => x.Value).Take(3).Select(x => $"• {x.Key}: {x.Value:N0} VND"))}",
                    MessageType = "summary",
                    Data = summary
                };

            case "/advice":
                var advice = await GeneratePersonalizedAdvice(userId);
                return new ChatResponseDto
                {
                    Response = advice,
                    MessageType = "advice"
                };

            case "/suggest":
                var suggestion = await GenerateSmartSuggestions(userId);
                return new ChatResponseDto
                {
                    Response = suggestion,
                    MessageType = "suggestion"
                };

            default:
                // Check if it's an /add command
                if (command.StartsWith("/add"))
                {
                    return await ProcessAddExpense(userId, command);
                }
                
                return new ChatResponseDto
                {
                    Response = "❓ Lệnh không được hỗ trợ. Các lệnh có sẵn: /summary, /advice, /suggest, /add",
                    MessageType = "error"
                };
        }
    }

    private async Task<ChatResponseDto?> ProcessNaturalLanguage(int userId, string message)
    {
        var lowerMessage = message.ToLower();
        _logger.LogInformation("🔍 Processing natural language: {Message}", message);

        // 1. MULTIPLE EXPENSES DETECTION - Phát hiện nhiều giao dịch
        var multipleExpenses = DetectMultipleExpenses(message);
        if (multipleExpenses != null && multipleExpenses.Count > 0)
        {
            _logger.LogInformation("💰 Multiple expenses detected: {Count} transactions", multipleExpenses.Count);
            return await ProcessMultipleExpenses(userId, multipleExpenses);
        }

        // 2. SINGLE EXPENSE DETECTION - Phát hiện 1 chi tiêu
        var expenseInfo = DetectExpenseIntent(message);
        if (expenseInfo.HasValue)
        {
            var (category, amount, note) = expenseInfo.Value;
            _logger.LogInformation("✅ Expense detected: Category={Category}, Amount={Amount}, Note={Note}", category, amount, note);
            return await ProcessAddExpense(userId, $"/add {category} {amount} {note}");
        }

        // 3. ACTIVITY DETECTION - Phát hiện hoạt động hàng ngày
        var activityResponse = DetectActivityIntent(message);
        if (activityResponse != null)
        {
            _logger.LogInformation("🎯 Activity detected: {Activity}", activityResponse);
            return new ChatResponseDto
            {
                Response = activityResponse,
                MessageType = "activity_suggestion"
            };
        }

        // 4. SUMMARY REQUESTS - Yêu cầu tổng hợp
        if (lowerMessage.Contains("tổng chi tiêu") || lowerMessage.Contains("chi tiêu tháng") || lowerMessage.Contains("bao nhiêu"))
        {
            return await ProcessSpecialCommand(userId, "/summary");
        }

        // 5. ANALYSIS REQUESTS - Yêu cầu phân tích
        if (lowerMessage.Contains("phân tích") || lowerMessage.Contains("đánh giá"))
        {
            return await ProcessSpecialCommand(userId, "/advice");
        }

        // 6. SUGGESTION REQUESTS - Yêu cầu gợi ý
        if (lowerMessage.Contains("gợi ý") || lowerMessage.Contains("tư vấn") || lowerMessage.Contains("suggest"))
        {
            return await ProcessSpecialCommand(userId, "/suggest");
        }

        return null; // No intent detected, use normal AI
    }

    private List<(string category, int amount, string note)>? DetectMultipleExpenses(string message)
    {
        try
        {
            // Split by newlines to get individual expense lines
            var lines = message.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var expenses = new List<(string category, int amount, string note)>();

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmedLine)) continue;

                // Try to extract expense from each line
                var expenseInfo = ExtractExpenseFromMessage(trimmedLine);
                if (expenseInfo.HasValue)
                {
                    expenses.Add(expenseInfo.Value);
                }
            }

            return expenses.Count > 0 ? expenses : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting multiple expenses");
            return null;
        }
    }

    private async Task<ChatResponseDto> ProcessMultipleExpenses(int userId, List<(string category, int amount, string note)> expenses)
    {
        try
        {
            var addedExpenses = new List<string>();
            int totalAmount = 0;

            foreach (var (category, amount, note) in expenses)
            {
                // Create transaction
                var transaction = new Transaction
                {
                    UserId = userId,
                    Amount = amount,
                    Type = "expense",
                    Category = category,
                    Description = note,
                    TransactionDate = DateTime.UtcNow
                };

                _context.Transactions.Add(transaction);
                addedExpenses.Add($"• {note}: {amount:N0}₫ ({category})");
                totalAmount += amount;
            }

            await _context.SaveChangesAsync();

            var responseText = $@"✅ Đã ghi nhận {expenses.Count} giao dịch:

{string.Join("\n", addedExpenses)}

💰 Tổng chi tiêu: {totalAmount:N0}₫";

            return new ChatResponseDto
            {
                Response = responseText,
                MessageType = "multiple_expenses_success",
                Data = new { count = expenses.Count, total = totalAmount }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing multiple expenses");
            return new ChatResponseDto
            {
                Response = "❌ Có lỗi xảy ra khi thêm các giao dịch. Vui lòng thử lại.",
                MessageType = "error"
            };
        }
    }

    private (string category, int amount, string note)? DetectExpenseIntent(string message)
    {
        var lowerMessage = message.ToLower();

        // Expense triggers - expanded to include more Vietnamese patterns
        var expenseTriggers = new[]
        {
            "tôi đã chi", "chi tiêu", "hết", "mua", "ăn", "uống", "xem phim", 
            "shopping", "đi chơi", "cafe", "grab", "taxi", "xăng", "học phí",
            "tiền", "xe", "tàu", "metro", "bánh", "trưa", "sáng", "tối", "đi học", "về"
        };

        var hasExpenseTrigger = expenseTriggers.Any(trigger => lowerMessage.Contains(trigger));
        var hasAmount = lowerMessage.Contains("k") || System.Text.RegularExpressions.Regex.IsMatch(lowerMessage, @"\d+");

        if (hasExpenseTrigger && hasAmount)
        {
            return ExtractExpenseFromMessage(message);
        }

        return null;
    }

    private string? DetectActivityIntent(string message)
    {
        var lowerMessage = message.ToLower();

        // Activity questions
        if (lowerMessage.Contains("hôm nay làm gì") || lowerMessage.Contains("hôm nay đi đâu"))
        {
            return @"🎯 **Gợi ý hoạt động hôm nay:**

💰 **Tiết kiệm:**
• Nấu ăn tại nhà thay vì đi nhà hàng
• Đi bộ hoặc xe đạp thay vì Grab
• Xem phim miễn phí trên YouTube/Netflix

🎉 **Giải trí vừa túi tiền:**
• Đi công viên, hồ Gươm (miễn phí)
• Cafe bình dân 20-30k
• Xem phim rạp vào khung giờ ưu đãi

📚 **Phát triển bản thân:**
• Đọc sách ở thư viện
• Học skill mới online
• Tập thể dục tại nhà

Bạn muốn làm gì cụ thể? Mình sẽ tính chi phí giúp bạn! 💡";
        }

        if (lowerMessage.Contains("cuối tuần") || lowerMessage.Contains("thứ 7") || lowerMessage.Contains("chủ nhật"))
        {
            return @"🌟 **Gợi ý cuối tuần tiết kiệm:**

🏠 **Tại nhà (0-50k):**
• Netflix chill + nấu ăn ngon
• Dọn dẹp nhà cửa
• Học kỹ năng mới online

🌳 **Ngoài trời (50-200k):**
• Picnic công viên + đồ ăn tự làm
• Chạy bộ/đạp xe quanh hồ
• Chụp ảnh street style

👥 **Cùng bạn bè (100-300k):**
• Karaoke mini (50-80k/người)
• Ăn lẩu tại nhà (100k/người)
• Board game cafe

Ngân sách cuối tuần của bạn là bao nhiêu? 💰";
        }

        if (lowerMessage.Contains("ăn gì") || lowerMessage.Contains("ăn ở đâu"))
        {
            return @"🍜 **Gợi ý ăn uống tiết kiệm:**

💰 **Tiết kiệm (20-50k):**
• Cơm bình dân: 25-35k
• Bánh mì pate: 15-25k
• Phở bò: 35-45k

⭐ **Trung bình (50-100k):**
• Lẩu mini 1 người: 60-80k
• Cơm niêu Singapore: 70-90k
• Bún bò Huế: 45-65k

🎉 **Chất lượng (100-200k):**
• Buffet lẩu: 150-180k
• Nhà hàng Âu: 120-200k
• Hot pot premium: 160-220k

Ngân sách ăn hôm nay là bao nhiêu? Mình gợi ý cụ thể! 😋";
        }

        if (lowerMessage.Contains("đi đâu chơi") || lowerMessage.Contains("địa điểm"))
        {
            return @"🗺️ **Địa điểm vui chơi theo ngân sách:**

🆓 **Miễn phí:**
• Hồ Gươm, Hồ Tây
• Phố cổ Hà Nội
• Công viên Thống Nhất
• Chùa Một Cột, Văn Miếu

💰 **50-100k:**
• Museum: 30-50k
• Cafe view đẹp: 40-80k
• Rạp chiếu phim sớm: 60-80k

🎯 **100-300k:**
• Sky bar: 150-250k/drink
• Spa mini: 200-300k
• Game center: 100-200k

📍 **Ở xa (200-500k):**
• Sapa 1 ngày: 400-600k
• Hạ Long: 500-800k
• Tam Đảo: 300-500k

Bạn có ngân sách bao nhiêu? 🎒";
        }

        if (lowerMessage.Contains("mua gì") || lowerMessage.Contains("shopping"))
        {
            return @"🛍️ **Shopping thông minh:**

🎯 **Cần thiết trước:**
• Quần áo cơ bản thiếu gì?
• Đồ dùng hàng ngày còn không?
• Sách/khóa học đầu tư bản thân

💡 **Nguyên tắc 24h:**
Muốn mua gì → chờ 24h → vẫn muốn mới mua

🔥 **Sale hunting:**
• Shopee/Tiki cuối tháng
• Outlet stores
• Second-hand chất lượng

💰 **Ngân sách:**
• Cần thiết: 70% ngân sách
• Muốn có: 20% ngân sách  
• Dự phòng: 10% ngân sách

Bạn đang muốn mua gì? Mình tư vấn có nên mua không! 🤔";
        }

        return null;
    }

    private (string category, int amount, string note)? ExtractExpenseFromMessage(string message)
    {
        try
        {
            // Enhanced pattern matching for Vietnamese - supports multiple formats
            var patterns = new[]
            {
                // "tiền xe xanh sm đi học 115k" - Most common Vietnamese pattern
                @"tiền\s+(.+?)\s+(\d+)k?",
                // "xe xanh sm đi học 115k"
                @"^(.+?)\s+(\d+)k?$",
                // "Tôi đã chi 100k cho bữa trưa"
                @"chi\s+(\d+)k?\s+cho\s+(.+)",
                // "100k cho bữa trưa" 
                @"(\d+)k?\s+cho\s+(.+)",
                // "xem phim hết 250k", "mua đồ hết 100k"
                @"(.+?)\s+hết\s+(\d+)k?",
                // "hôm nay tôi xem phim 250k"
                @"(xem\s+phim|ăn\s+[^.!?\n]*|mua\s+[^.!?\n]*)\s+(\d+)k?",
                // "mua [something] 100k"
                @"mua\s+(.+?)\s+(\d+)k?",
                // "ăn trưa 50k", "cafe 39k"
                @"(ăn\s+.+?|cafe)\s+(\d+)k?",
            };

            foreach (var pattern in patterns)
            {
                var match = System.Text.RegularExpressions.Regex.Match(message.ToLower().Trim(), pattern);
                if (match.Success)
                {
                    string amountStr, categoryStr;
                    
                    // Handle different group orders based on pattern
                    if (pattern.Contains("tiền\\s+") || pattern.StartsWith("^") || 
                        pattern.Contains("hết") || pattern.Contains("ăn") || 
                        pattern.Contains("xem") || pattern.Contains("mua"))
                    {
                        // Pattern: "description amount" or "activity hết amount"
                        categoryStr = match.Groups[1].Value.Trim();
                        amountStr = match.Groups[2].Value;
                    }
                    else
                    {
                        // Pattern: "amount cho activity" 
                        amountStr = match.Groups[1].Value;
                        categoryStr = match.Groups[2].Value.Trim();
                    }

                    if (int.TryParse(amountStr, out int amount))
                    {
                        // Convert "k" to thousands (already in thousands)
                        if (message.ToLower().Contains($"{amountStr}k"))
                        {
                            amount *= 1000;
                        }

                        // Determine category
                        var category = DetermineCategory(categoryStr);
                        
                        _logger.LogInformation("💰 Extracted expense: Category={Category}, Amount={Amount}, Note={Note}", 
                            category, amount, categoryStr);
                        
                        return (category, amount, categoryStr);
                    }
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting expense from message");
            return null;
        }
    }

    private string DetermineCategory(string text)
    {
        text = text.ToLower();
        
        // Ăn uống - Food & Beverage
        if (text.Contains("ăn") || text.Contains("trưa") || text.Contains("tối") || 
            text.Contains("sáng") || text.Contains("cơm") || text.Contains("cafe") || 
            text.Contains("bánh") || text.Contains("uống"))
            return "Ăn uống";
            
        // Di chuyển - Transportation  
        if (text.Contains("xe") || text.Contains("xăng") || text.Contains("grab") || 
            text.Contains("bus") || text.Contains("taxi") || text.Contains("xanh sm") || 
            text.Contains("tàu") || text.Contains("metro") || text.Contains("đi học") || 
            text.Contains("về"))
            return "Di chuyển";
            
        // Mua sắm - Shopping
        if (text.Contains("mua") || text.Contains("quần áo") || text.Contains("shopping") || 
            text.Contains("cửa hàng") || text.Contains("tiện lợi"))
            return "Mua sắm";
            
        // Giải trí - Entertainment
        if (text.Contains("phim") || text.Contains("game") || text.Contains("giải trí") || 
            text.Contains("chơi"))
            return "Giải trí";
            
        // Học tập - Education
        if (text.Contains("học") || text.Contains("sách") || text.Contains("khóa học"))
            return "Học tập";
        
        return "Khác";
    }

    private async Task<DetailedFinancialSummaryDto> GetUserFinancialContext(int userId)
    {
        var currentMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        // Get transactions this month
        var transactions = await _context.Transactions
            .Where(t => t.UserId == userId && t.TransactionDate >= currentMonth)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();

        var income = transactions.Where(t => t.Type == "income").Sum(t => t.Amount);
        var expenses = transactions.Where(t => t.Type == "expense").Sum(t => t.Amount);

        var categoryBreakdown = transactions
            .Where(t => t.Type == "expense")
            .GroupBy(t => t.Category ?? "Khác")
            .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));

        var recentExpenses = transactions
            .Where(t => t.Type == "expense")
            .Take(10)
            .Select(t => new ExpenseDataDto
            {
                Category = t.Category ?? "Khác",
                Amount = t.Amount,
                Note = t.Description ?? "",
                Date = t.TransactionDate
            })
            .ToList();

        // Get user's current balance
        var user = await _context.Users.FindAsync(userId);
        var balance = user?.AvailableBalance ?? 0;

        return new DetailedFinancialSummaryDto
        {
            TotalIncome = income,
            TotalExpenses = expenses,
            Balance = balance,
            CategoryBreakdown = categoryBreakdown,
            RecentExpenses = recentExpenses
        };
    }

    private async Task<string> GeneratePersonalizedAdvice(int userId)
    {
        var context = await GetUserFinancialContext(userId);
        
        if (context.TotalExpenses == 0)
        {
            return "🌟 Bạn chưa có giao dịch nào tháng này. Hãy bắt đầu ghi chép chi tiêu để nhận được tư vấn cá nhân hóa!";
        }

        var topCategory = context.CategoryBreakdown.OrderByDescending(x => x.Value).FirstOrDefault();
        var savingsRate = context.TotalIncome > 0 ? ((context.TotalIncome - context.TotalExpenses) / context.TotalIncome) * 100 : 0;

        var advice = $@"💡 **Phân tích tài chính cá nhân:**

📊 Tỷ lệ tiết kiệm: {savingsRate:F1}%
{(savingsRate >= 20 ? "✅ Tuyệt vời!" : savingsRate >= 10 ? "⚠️ Tạm ổn, có thể cải thiện" : "🚨 Cần cải thiện ngay")}

🔥 Chi tiêu nhiều nhất: {topCategory.Key} ({topCategory.Value:N0} VND)

💰 **Gợi ý cải thiện:**
{(savingsRate < 10 ? "• Cắt giảm 10-15% chi tiêu không cần thiết\n" : "")}• Theo dõi chi tiêu hàng ngày
• Đặt ngân sách cho từng danh mục
• Tìm cách tăng thu nhập phụ";

        return advice;
    }

    private async Task<string> GenerateSmartSuggestions(int userId)
    {
        try
        {
            var summary = await GetUserFinancialContext(userId);
            var savingsRate = summary.TotalIncome > 0 ? ((summary.TotalIncome - summary.TotalExpenses) / summary.TotalIncome) * 100 : 0;

            if (summary.TotalExpenses == 0)
            {
                return "Hãy nhập vài khoản chi để mình tư vấn nhé! 💡";
            }
            else if (savingsRate > 20)
            {
                return "Chi tiêu của bạn rất tốt! Hãy duy trì mức tiết kiệm này 💰\n\n🎯 Gợi ý:\n• Đầu tư phần tiết kiệm vào quỹ an toàn\n• Tăng emergency fund lên 6 tháng chi tiêu";
            }
            else if (savingsRate > 10)
            {
                return "Bạn đang chi tiêu ổn, nhưng có thể tối ưu thêm ở phần giải trí hoặc ăn uống 🍜\n\n💡 Gợi ý:\n• Cắt giảm 10-15% chi tiêu không cần thiết\n• Theo dõi chi tiêu hàng ngày\n• Đặt ngân sách cho từng danh mục";
            }
            else
            {
                return "⚠️ Chi tiêu đang cao, nên xem xét lại ngân sách và cắt giảm các khoản không cần thiết.\n\n🚨 Hành động ngay:\n• Liệt kê chi tiêu thiết yếu vs không thiết yếu\n• Cắt giảm 30% chi tiêu giải trí\n• Tìm nguồn thu nhập phụ";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating suggestions");
            return "💡 Gợi ý chung:\n• Lập ngân sách hàng tháng\n• Theo dõi chi tiêu hàng ngày\n• Tiết kiệm ít nhất 20% thu nhập\n• Đầu tư vào quỹ an toàn";
        }
    }

    private async Task<string> CallGroqAPI(string systemPrompt, string userMessage)
    {
        try
        {
            _logger.LogInformation("🤖 Calling Groq API with prompt length: {PromptLength}, message: {Message}", 
                systemPrompt.Length, userMessage);

            var payload = new
            {
                model = "llama3-8b-8192",
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userMessage }
                },
                temperature = 0.5,
                max_tokens = 1024,
                top_p = 0.9
            };

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_groqApiKey}");

            var jsonContent = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _logger.LogInformation("📤 Sending request to Groq API...");
            var response = await _httpClient.PostAsync("https://api.groq.com/openai/v1/chat/completions", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("📥 Groq API Response - Status: {StatusCode}, Content Length: {Length}", 
                response.StatusCode, responseContent.Length);

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                var aiResponse = result.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "Xin lỗi, tôi không thể xử lý yêu cầu này.";
                
                _logger.LogInformation("✅ AI Response received: {ResponseLength} characters", aiResponse.Length);
                return aiResponse;
            }
            else
            {
                _logger.LogError("❌ Groq API error: {StatusCode} - {Content}", response.StatusCode, responseContent);
                return "🤖 Xin lỗi, hệ thống AI tạm thời gặp sự cố. Vui lòng thử lại sau.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error calling Groq API: {Message}", ex.Message);
            return "🤖 Xin lỗi, tôi đang gặp sự cố kỹ thuật. Vui lòng thử lại sau.";
        }
    }

    // GET: api/Chat/setup
    // Auto-create ChatMessages table if it doesn't exist
    [HttpGet("setup")]
    public async Task<ActionResult<object>> SetupChatTable()
    {
        try
        {
            _logger.LogInformation("🔧 Setting up ChatMessages table...");

            // Check if table exists by trying to query it
            try
            {
                var testQuery = await _context.ChatMessages.CountAsync();
                _logger.LogInformation("✅ ChatMessages table already exists with {Count} records", testQuery);
                return Ok(new { message = "ChatMessages table already exists", recordCount = testQuery });
            }
            catch (Exception tableNotExistsEx)
            {
                _logger.LogInformation("📋 ChatMessages table doesn't exist, creating it...");
                _logger.LogInformation("Exception details: {Exception}", tableNotExistsEx.Message);
                
                // Detect database provider
                var connectionString = _context.Database.GetConnectionString();
                var isPostgreSQL = connectionString?.Contains("Host=") == true || connectionString?.Contains("PostgreSQL") == true;
                
                string createTableSql;
                
                if (isPostgreSQL)
                {
                    // PostgreSQL syntax
                    createTableSql = @"
                        CREATE TABLE IF NOT EXISTS ""ChatMessages"" (
                            ""Id"" SERIAL PRIMARY KEY,
                            ""UserId"" INTEGER NOT NULL,
                            ""Message"" VARCHAR(1000) NOT NULL,
                            ""Response"" VARCHAR(2000),
                            ""CreatedAt"" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                            ""MessageType"" VARCHAR(20) NOT NULL DEFAULT 'user',
                            CONSTRAINT ""FK_ChatMessages_Users_UserId"" FOREIGN KEY (""UserId"") REFERENCES ""Users"" (""Id"") ON DELETE CASCADE
                        );
                        
                        CREATE INDEX IF NOT EXISTS ""IX_ChatMessages_UserId_CreatedAt"" ON ""ChatMessages"" (""UserId"", ""CreatedAt"");";
                }
                else
                {
                    // SQL Server syntax
                    createTableSql = @"
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ChatMessages' AND xtype='U')
                        BEGIN
                            CREATE TABLE [ChatMessages] (
                                [Id] int NOT NULL IDENTITY(1,1),
                                [UserId] int NOT NULL,
                                [Message] nvarchar(1000) NOT NULL,
                                [Response] nvarchar(2000) NULL,
                                [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
                                [MessageType] nvarchar(20) NOT NULL DEFAULT 'user',
                                CONSTRAINT [PK_ChatMessages] PRIMARY KEY ([Id]),
                                CONSTRAINT [FK_ChatMessages_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
                            );
                            
                            CREATE INDEX [IX_ChatMessages_UserId_CreatedAt] ON [ChatMessages] ([UserId], [CreatedAt]);
                        END";
                }

                await _context.Database.ExecuteSqlRawAsync(createTableSql);
                _logger.LogInformation("✅ ChatMessages table created successfully");
                
                return Ok(new { message = "ChatMessages table created successfully", databaseType = isPostgreSQL ? "PostgreSQL" : "SQL Server" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error setting up ChatMessages table");
            return StatusCode(500, new { message = "Error setting up ChatMessages table", error = ex.Message });
        }
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            _logger.LogError("Unable to extract user ID from JWT token");
            throw new UnauthorizedAccessException("Invalid or missing user ID in token");
        }
        return userId;
    }

    private async Task<ChatResponseDto> ProcessAddExpense(int userId, string command)
    {
        try
        {
            // Parse: /add category amount note
            var parts = command.Split(' ', 4);
            if (parts.Length < 3)
            {
                return new ChatResponseDto
                {
                    Response = "❗ Cú pháp sai. Dạng đúng: /add <loại> <số tiền> <ghi chú>",
                    MessageType = "error"
                };
            }

            var category = parts[1];
            if (!decimal.TryParse(parts[2], out decimal amount))
            {
                return new ChatResponseDto
                {
                    Response = "❗ Số tiền không hợp lệ. Vui lòng nhập số.",
                    MessageType = "error"
                };
            }

            var note = parts.Length > 3 ? parts[3] : "";

            // Create transaction
            var transaction = new Transaction
            {
                UserId = userId,
                Amount = amount,
                Type = "expense",
                Category = category,
                Description = note,
                TransactionDate = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return new ChatResponseDto
            {
                Response = $"✅ Đã ghi nhận {amount:N0}₫ cho {category} ({note})",
                MessageType = "success"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing add expense command");
            return new ChatResponseDto
            {
                Response = "❌ Có lỗi xảy ra khi thêm chi tiêu. Vui lòng thử lại.",
                MessageType = "error"
            };
        }
    }
}
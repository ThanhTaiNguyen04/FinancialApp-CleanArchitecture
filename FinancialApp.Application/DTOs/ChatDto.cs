namespace FinancialApp.Application.DTOs
{
    public class ChatMessageDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string MessageType { get; set; } = "text"; // text, expense, summary, suggestion
    }

    public class SendChatMessageDto
    {
        public string Message { get; set; } = string.Empty;
    }

    public class ChatResponseDto
    {
        public string Response { get; set; } = string.Empty;
        public string MessageType { get; set; } = "text";
        public object? Data { get; set; } // For structured data like expenses, summaries
    }

    public class ExpenseDataDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Note { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }

    public class DetailedFinancialSummaryDto
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal Balance { get; set; }
        public List<ExpenseDataDto> RecentExpenses { get; set; } = new();
        public Dictionary<string, decimal> CategoryBreakdown { get; set; } = new();
    }
}
using FinancialApp.Domain.Entities;

namespace FinancialApp.Domain.Entities
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string MessageType { get; set; } = "text"; // text, expense, summary, suggestion

        // Navigation properties
        public User User { get; set; } = null!;
    }
}
namespace FinancialApp.Domain.Entities
{
    public class PremiumRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? RejectionReason { get; set; }
        public string? TransactionReference { get; set; }
        public decimal Amount { get; set; } = 29000;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual User? User { get; set; }
        public virtual User? ApprovedByUser { get; set; }
    }
}

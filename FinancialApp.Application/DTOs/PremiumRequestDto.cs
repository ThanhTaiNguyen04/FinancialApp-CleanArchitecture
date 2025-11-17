namespace FinancialApp.Application.DTOs
{
    public class PremiumRequestDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; } = "Pending";
        public int? ApprovedBy { get; set; }
        public string? ApprovedByName { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? RejectionReason { get; set; }
        public string? TransactionReference { get; set; }
        public decimal Amount { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreatePremiumRequestDto
    {
        public string? TransactionReference { get; set; }
        public string? Notes { get; set; }
    }

    public class ApprovePremiumRequestDto
    {
        public bool Approved { get; set; }
        public string? RejectionReason { get; set; }
    }

    public class RevokePremiumRequestDto
    {
        public string? Reason { get; set; }
    }
}

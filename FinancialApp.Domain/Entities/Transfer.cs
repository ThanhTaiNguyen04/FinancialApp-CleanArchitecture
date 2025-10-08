using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialApp.Domain.Entities;

public class Transfer
{
    [Key]
    public int Id { get; set; }
    
    public int FromUserId { get; set; }
    public int ToContactId { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [Required]
    [StringLength(20)]
    public string TransferType { get; set; } = string.Empty; // 'mobile', 'qr_code', 'card', 'utilities'
    
    [Required]
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "pending"; // 'pending', 'completed', 'failed'
    
    public DateTime TransferDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation Properties
    [ForeignKey("FromUserId")]
    public User FromUser { get; set; } = null!;
    
    [ForeignKey("ToContactId")]
    public Contact ToContact { get; set; } = null!;
}
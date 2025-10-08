using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialApp.Domain.Entities;

public class SavingGoal
{
    [Key]
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty; // Giáo dục, Mua nhà, Du lịch
    
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal TargetAmount { get; set; } // Số tiền mục tiêu
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal CurrentAmount { get; set; } = 0; // Số tiền hiện tại
    
    public DateTime TargetDate { get; set; } // Ngày dự kiến hoàn thành
    
    [StringLength(50)]
    public string IconName { get; set; } = string.Empty;
    
    [StringLength(20)]
    public string ColorCode { get; set; } = string.Empty;
    
    [StringLength(20)]
    public string Status { get; set; } = "active"; // active, completed, paused
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation Properties
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
    
    // Computed Properties
    [NotMapped]
    public decimal RemainingAmount => TargetAmount - CurrentAmount;
    
    [NotMapped]
    public decimal PercentageCompleted => TargetAmount > 0 ? (CurrentAmount / TargetAmount) * 100 : 0;
    
    [NotMapped]
    public int DaysRemaining => (TargetDate - DateTime.UtcNow).Days;
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialApp.Domain.Entities;

public class Budget
{
    [Key]
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public int CategoryId { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal BudgetAmount { get; set; } // Ngân sách dự kiến
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal SpentAmount { get; set; } = 0; // Đã chi tiêu
    
    public int Month { get; set; } // Tháng (1-12)
    public int Year { get; set; }  // Năm
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation Properties
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
    
    [ForeignKey("CategoryId")]
    public Category Category { get; set; } = null!;
    
    // Computed Properties
    [NotMapped]
    public decimal RemainingAmount => BudgetAmount - SpentAmount;
    
    [NotMapped]
    public decimal PercentageUsed => BudgetAmount > 0 ? (SpentAmount / BudgetAmount) * 100 : 0;
}
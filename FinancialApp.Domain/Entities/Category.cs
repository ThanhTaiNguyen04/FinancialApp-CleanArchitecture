using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialApp.Domain.Entities;

public class Category
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty; // Lương, Di chuyển, Thuê nhà, Y tế
    
    [StringLength(50)]
    public string IconName { get; set; } = string.Empty; // Icon identifier
    
    [StringLength(20)]
    public string ColorCode { get; set; } = string.Empty; // Hex color code
    
    [Required]
    [StringLength(20)]
    public string Type { get; set; } = string.Empty; // "income" or "expense"
    
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation Properties
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
}
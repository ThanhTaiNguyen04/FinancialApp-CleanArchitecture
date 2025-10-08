using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialApp.Domain.Entities;

public class Contact
{
    [Key]
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string AvatarUrl { get; set; } = string.Empty;
    
    public bool IsRecent { get; set; } = false;
    public DateTime? LastTransactionDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation Properties
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
    
    public ICollection<Transfer> ReceivedTransfers { get; set; } = new List<Transfer>();
}
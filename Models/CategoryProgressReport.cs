using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WordRace000.Models
{
    public class CategoryProgressReport
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public DateTime GeneratedAt { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal OverallProgress { get; set; }
        
        [Required]
        public int TotalLearnedWords { get; set; }
        
        [Required]
        public int TotalWords { get; set; }

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual ICollection<CategoryProgressDetail> Details { get; set; } = new List<CategoryProgressDetail>();
    }

    public class CategoryProgressDetail
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int ReportId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;
        
        [Required]
        public int TotalWords { get; set; }
        
        [Required]
        public int LearnedWords { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal ProgressPercentage { get; set; }
        
        [Required]
        public DateTime LastUpdated { get; set; }

        // Navigation property
        public virtual CategoryProgressReport Report { get; set; } = null!;
    }
} 
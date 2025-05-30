using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WordRace000.Models
{
    public class AnalysisReport
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime ReportDate { get; set; }
        
        [Required]
        public string? ReportContent { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public AnalysisReport()
        {
            ReportDate = DateTime.UtcNow;
        }
    }
}
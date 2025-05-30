using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WordRace000.Models
{
    [Table("WordLogs")]
    public class WordLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int WordId { get; set; }
        public DateTime LogDate { get; set; }
        public bool IsCorrect { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("WordId")]
        public virtual Word? Word { get; set; }

        public WordLog()
        {
            LogDate = DateTime.UtcNow;
        }
    }
} 
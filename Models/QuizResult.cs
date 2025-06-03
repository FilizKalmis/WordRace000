using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WordRace000.Models
{
    public class QuizResult
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public int UserId { get; set; }
        public int Score { get; set; }
        public DateTime CompletedAt { get; set; }

        // Navigation properties
        [ForeignKey("QuizId")]
        public virtual Quiz? Quiz { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public QuizResult()
        {
            CompletedAt = DateTime.UtcNow;
        }
    }
} 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WordRace000.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Score { get; set; }
        public bool IsCompleted { get; set; }

        // Navigation properties
        public virtual User? User { get; set; }
        public virtual ICollection<QuizDetail> QuizDetails { get; set; }

        public Quiz()
        {
            QuizDetails = new HashSet<QuizDetail>();
            CreatedAt = DateTime.UtcNow;
            Score = 0;
            IsCompleted = false;
        }
    }
} 
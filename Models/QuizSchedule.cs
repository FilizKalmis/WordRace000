using System.ComponentModel.DataAnnotations.Schema;

namespace WordRace000.Models
{
    public class QuizSchedule
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int WordId { get; set; }
        public DateTime NextTestDate { get; set; }
        public int AttemptCount { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime LastUpdateAt { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("WordId")]
        public virtual Word? Word { get; set; }

        public QuizSchedule()
        {
            CreateAt = DateTime.UtcNow;
            LastUpdateAt = DateTime.UtcNow;
            AttemptCount = 0;
            IsCompleted = false;
        }
    }
} 
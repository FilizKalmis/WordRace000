using System.ComponentModel.DataAnnotations.Schema;

namespace WordRace000.Models
{
    [Table("QuizSchedule")]
    public class QuizSchedule
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int WordId { get; set; }
        public DateTime NextTestDate { get; set; }
        public int? AttemptCount { get; set; }
        public bool? IsCompleted { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("WordId")]
        public virtual Word? Word { get; set; }

        public QuizSchedule()
        {
            CreatedAt = DateTime.UtcNow;
            LastUpdatedAt = DateTime.UtcNow;
            AttemptCount = 0;
            IsCompleted = false;
        }
    }
} 
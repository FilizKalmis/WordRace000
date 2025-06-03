using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WordRace000.Models
{
    [Table("WordProgress")]
    public class WordProgress
    {
        public int Id { get; set; }
        public int WordId { get; set; }
        public int UserId { get; set; }
        public int Progress { get; set; }
        public DateTime LastAttemptDate { get; set; }
        public DateTime NextTestDate { get; set; }
        public bool IsLearned { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdateAt { get; set; }
        public int CorrectAttempts { get; set; }
        public int TotalAttempts { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("WordId")]
        public virtual Word? Word { get; set; }

        public WordProgress()
        {
            CreatedAt = DateTime.UtcNow;
            LastUpdateAt = DateTime.UtcNow;
            LastAttemptDate = DateTime.UtcNow;
            NextTestDate = DateTime.UtcNow.AddDays(1); // Varsayılan olarak bir gün sonra
            Progress = 0;
            IsLearned = false;
            CorrectAttempts = 0;
            TotalAttempts = 0;
        }
    }
}
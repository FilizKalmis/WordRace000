using System.ComponentModel.DataAnnotations.Schema;

namespace WordRace000.Models
{
    public class WordleGame
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int WordId { get; set; }
        public int AttemptCount { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsSuccessful { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("WordId")]
        public virtual Word? Word { get; set; }

        public WordleGame()
        {
            CreatedAt = DateTime.UtcNow;
            AttemptCount = 0;
            IsCompleted = false;
            IsSuccessful = false;
        }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WordRace000.Models
{
    [Table("WordleGame")]
    public class WordleGame
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int WordId { get; set; }

        public int AttemptCount { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsSuccessful { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        [MaxLength(500)]
        public string? Guesses { get; set; }

        [MaxLength(500)]
        public string? Feedbacks { get; set; }

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
using System.ComponentModel.DataAnnotations.Schema;

namespace WordRace000.Models
{
    public class LearnedWord
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int WordId { get; set; }
        public DateTime LearnedDate { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("WordId")]
        public virtual Word? Word { get; set; }

        public LearnedWord()
        {
            LearnedDate = DateTime.UtcNow;
        }
    }
}
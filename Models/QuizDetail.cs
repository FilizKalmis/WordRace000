using System.ComponentModel.DataAnnotations.Schema;

namespace WordRace000.Models
{
    public class QuizDetail
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public int WordId { get; set; }
        public bool IsCorrect { get; set; }
        public bool IsAnswered { get; set; }

        [ForeignKey("QuizId")]
        public virtual Quiz? Quiz { get; set; }

        [ForeignKey("WordId")]
        public virtual Word? Word { get; set; }
    }
}
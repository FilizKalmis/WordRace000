using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WordRace000.Models
{
    [Table("Words")]
    public class Word
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("EnglishWord")]
        public string English { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("TurkishMeaning")]
        public string Turkish { get; set; }

        public string? Example { get; set; }
        public string? ImageUrl { get; set; }
        public string? AudioUrl { get; set; }
        public int Difficulty { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        // Navigation properties
        public virtual ICollection<WordSampleWord> WordSampleWords { get; set; }
        public virtual ICollection<LearnedWord> LearnedWords { get; set; }
        public virtual ICollection<WordLog> WordLogs { get; set; }
        public virtual ICollection<QuizDetail> QuizDetails { get; set; }
        public virtual ICollection<QuizSchedule> QuizSchedules { get; set; }
        public virtual ICollection<WordleGame> WordleGames { get; set; }

        public Word()
        {
            WordSampleWords = new HashSet<WordSampleWord>();
            LearnedWords = new HashSet<LearnedWord>();
            WordLogs = new HashSet<WordLog>();
            QuizDetails = new HashSet<QuizDetail>();
            QuizSchedules = new HashSet<QuizSchedule>();
            WordleGames = new HashSet<WordleGame>();
            English = "";
            Turkish = "";
            Difficulty = 1;
        }
    }
} 
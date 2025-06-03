using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WordRace000.Models
{
    [Table("Words")]
    public class Word
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("EnglishWord")]
        public string English { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("TurkishMeaning")]
        public string Turkish { get; set; }

        [MaxLength(500)]
        public string? Example { get; set; }

        [MaxLength(255)]
        public string? ImageUrl { get; set; }

        [MaxLength(255)]
        public string? AudioUrl { get; set; }

        [Range(1, 5)]
        public int Difficulty { get; set; }

        [Required]
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
            English = string.Empty;
            Turkish = string.Empty;
            Difficulty = 1;
        }
    }
} 
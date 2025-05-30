using System.ComponentModel.DataAnnotations;

namespace WordRace000.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string? FirstName { get; set; }
        
        [Required]
        [StringLength(50)]
        public string? LastName { get; set; }

        [Required]
        [StringLength(50)]
        public string? Username { get; set; }
        
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }
        
        [Required]
        [StringLength(100)]
        public string? Password { get; set; }

        [StringLength(255)]
        public string? ProfilePicture { get; set; } 

         // Tam adı döndüren property
        public string FullName => $"{FirstName} {LastName}";       

        // Navigation properties
        public virtual ICollection<ExamResult> ExamResults { get; set; }
        public virtual ICollection<LearnedWord> LearnedWords { get; set; }
        public virtual ICollection<Quiz> Quizzes { get; set; }
        public virtual ICollection<QuizSchedule> QuizSchedules { get; set; }
        public virtual ICollection<Settings> Settings { get; set; }
        public virtual ICollection<WordChainStory> WordChainStories { get; set; }
        public virtual ICollection<WordleGame> WordleGames { get; set; }
        public virtual ICollection<WordLog> WordLogs { get; set; }
        public virtual ICollection<WordProgress> WordProgresses { get; set; }
        public virtual ICollection<AnalysisReport> AnalysisReports { get; set; }

        public User()
        {
            ExamResults = new HashSet<ExamResult>();
            LearnedWords = new HashSet<LearnedWord>();
            Quizzes = new HashSet<Quiz>();
            QuizSchedules = new HashSet<QuizSchedule>();
            Settings = new HashSet<Settings>();
            WordChainStories = new HashSet<WordChainStory>();
            WordleGames = new HashSet<WordleGame>();
            WordLogs = new HashSet<WordLog>();
            WordProgresses = new HashSet<WordProgress>();
            AnalysisReports = new HashSet<AnalysisReport>();
            
            // Default değerler
            FirstName = "";
            LastName = "";
            Username = "";
            Email = "";
            Password = "";
            ProfilePicture = "/images/default-avatar.png";
        }
    }
} 
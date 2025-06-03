using Microsoft.EntityFrameworkCore;
using WordRace000.Models;

namespace WordRace000.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Word> Words { get; set; }
        public DbSet<WordSample> WordSample { get; set; }
        public DbSet<WordSampleWord> WordSampleWords { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<QuizDetail> QuizDetails { get; set; }
        public DbSet<QuizSchedule> QuizSchedules { get; set; }
        public DbSet<LearnedWord> LearnedWords { get; set; }
        public DbSet<WordProgress> WordProgresses { get; set; }
        public DbSet<WordLog> WordLogs { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<AnalysisReport> AnalysisReports { get; set; }
        public DbSet<WordleGame> WordleGame { get; set; }
        public DbSet<WordChainStory> WordChainStories { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraints
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Configure WordProgress table name
            modelBuilder.Entity<WordProgress>()
                .ToTable("WordProgress");

            // Configure relationships
            modelBuilder.Entity<WordSampleWord>()
                .HasOne(wsw => wsw.Word)
                .WithMany(w => w.WordSampleWords)
                .HasForeignKey(wsw => wsw.WordId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WordSampleWord>()
                .HasOne(wsw => wsw.WordSample)
                .WithMany(ws => ws.WordSampleWords)
                .HasForeignKey(wsw => wsw.WordSampleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Add other relationship configurations as needed
        }
    }
} 
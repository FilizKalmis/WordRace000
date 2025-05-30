 using System.ComponentModel.DataAnnotations.Schema;

namespace WordRace000.Models
{
    public class ExamResult
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public int CorrectCount { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime ExamDate { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        public ExamResult()
        {
            ExamDate = DateTime.UtcNow;
        }
    }
}
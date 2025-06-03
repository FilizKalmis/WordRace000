using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WordRace000.Models
{
    [Table("Category")]
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string CategoryName { get; set; }

        // Navigation properties
        public virtual ICollection<Word> Words { get; set; }
        public virtual ICollection<ExamResult> ExamResults { get; set; }

        public Category()
        {
            Words = new HashSet<Word>();
            ExamResults = new HashSet<ExamResult>();
            CategoryName = string.Empty;
        }
    }
}
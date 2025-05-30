using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WordRace000.Models
{
    public class WordChainStory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        
        [Required]
        public string? StoryText { get; set; }
        public DateTime CreatedDate { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public WordChainStory()
        {
            CreatedDate = DateTime.UtcNow;
        }
    }
}
using System.ComponentModel.DataAnnotations;

namespace WordRace000.Models
{
    public class WordSample
    {
        public int Id { get; set; }

        [Required]
        public string SampleText { get; set; } = string.Empty;

        // Navigation property
        public virtual ICollection<WordSampleWord> WordSampleWords { get; set; }

        public WordSample()
        {
            WordSampleWords = new HashSet<WordSampleWord>();
        }
    }
} 
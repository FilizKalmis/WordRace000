using System;
using System.ComponentModel.DataAnnotations;

namespace WordRace000.Models
{
    public class WordSample
    {
        public int Id { get; set; }

        [Required]
        public string SampleText { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual ICollection<WordSampleWord> WordSampleWords { get; set; } = new List<WordSampleWord>();

        public WordSample()
        {
            WordSampleWords = new HashSet<WordSampleWord>();
        }
    }
} 
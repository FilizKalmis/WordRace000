using System;

namespace WordRace000.Models
{
    public class CategoryProgressViewModel
    {
        public string CategoryName { get; set; } = string.Empty;
        public int TotalWords { get; set; }
        public int LearnedWords { get; set; }
        public double ProgressPercentage => TotalWords > 0 ? (LearnedWords * 100.0 / TotalWords) : 0;
        public DateTime LastUpdated { get; set; }
    }
} 
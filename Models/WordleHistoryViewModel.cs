using System;
using System.Collections.Generic;

namespace WordRace000.Models
{
    public class WordleHistoryViewModel
    {
        public string CategoryName { get; set; }
        public string Word { get; set; }
        public int AttemptCount { get; set; }
        public bool IsSuccessful { get; set; }
        public DateTime? CompletedAt { get; set; }
        public List<string> Guesses { get; set; }

        public WordleHistoryViewModel()
        {
            CategoryName = string.Empty;
            Word = string.Empty;
            Guesses = new List<string>();
        }
    }
} 
using System;
using System.Collections.Generic;

namespace WordRace000.Models
{
    public class ProfileViewModel
    {
        public User User { get; set; } = null!;
        public int TotalWordsLearned { get; set; }
        public int QuizzesTaken { get; set; }
        public int WordsInProgress { get; set; }
        public int TotalWords { get; set; }
        public int LearnedWords { get; set; }
        public int InProgressWords { get; set; }
        public int TotalQuizzes { get; set; }
        public DateTime? LastQuizDate { get; set; }
        public List<CategoryProgressViewModel> CategoryProgress { get; set; } = new();
    }
} 
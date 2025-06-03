using System;

namespace WordRace000.Models
{
    public class WordleViewModel
    {
        public int GameId { get; set; }
        public string TargetWord { get; set; }
        public string CategoryName { get; set; }
        public int WordLength { get; set; }

        public WordleViewModel()
        {
            TargetWord = string.Empty;
            CategoryName = string.Empty;
        }
    }
} 
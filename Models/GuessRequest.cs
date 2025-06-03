using System.ComponentModel.DataAnnotations;

namespace WordRace000.Models
{
    public class GuessRequest
    {
        [Required]
        [StringLength(14)]
        public string Guess { get; set; }

        [Required]
        [StringLength(14)]
        public string TargetWord { get; set; }

        public int GameId { get; set; }

        public GuessRequest()
        {
            Guess = string.Empty;
            TargetWord = string.Empty;
        }
    }
} 
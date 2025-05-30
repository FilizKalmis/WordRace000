using System.ComponentModel.DataAnnotations.Schema;

namespace WordRace000.Models
{
    public class Settings
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int DailyWordCount { get; set; }
        public bool EmailNotifications { get; set; }
        public bool DarkMode { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public Settings()
        {
            DailyWordCount = 10; // Default value
            EmailNotifications = true; // Default value
            DarkMode = false; // Default value
        }
    }
}
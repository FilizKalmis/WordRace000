using System.ComponentModel.DataAnnotations.Schema;

namespace WordRace000.Models
{
    public class WordSampleWord
    {
        public int Id { get; set; }
        public int WordId { get; set; }
        public int WordSampleId { get; set; }

        public virtual Word Word { get; set; }
        public virtual WordSample WordSample { get; set; }
    }
}
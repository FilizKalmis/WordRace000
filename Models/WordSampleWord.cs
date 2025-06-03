using System.ComponentModel.DataAnnotations.Schema;

namespace WordRace000.Models
{
    public class WordSampleWord
    {
        public int Id { get; set; }
        public int WordId { get; set; }
        public int WordSampleId { get; set; }

        [ForeignKey("WordId")]
        public virtual Word? Word { get; set; }

        [ForeignKey("WordSampleId")]
        public virtual WordSample? WordSample { get; set; }
    }
}
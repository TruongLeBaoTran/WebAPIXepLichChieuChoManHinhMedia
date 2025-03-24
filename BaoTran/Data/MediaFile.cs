using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaoTran.Data
{
    public class MediaFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MediaFileId { get; set; }
        public string Title { get; set; }
        public string Singer { get; set; }
        public string Musician { get; set; }
        public string FileFormat { get; set; }
        public string Duration { get; set; }
        public string File { get; set; }
        public string FileType { get; set; }
        public ICollection<DateRange> DateRanges { get; set; } = new List<DateRange>();

    }

}

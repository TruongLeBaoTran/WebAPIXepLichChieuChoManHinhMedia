using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaoTran.Data
{
    public class DateRange
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DateRangeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MediaFileId { get; set; }
        public MediaFile MediaFile { get; set; }

        public ICollection<DayOfTheWeek> DayOfTheWeeks { get; set; } = new List<DayOfTheWeek>();

    }
}

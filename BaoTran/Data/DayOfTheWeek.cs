using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaoTran.Data
{
    public class DayOfTheWeek
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DayOfWeekId { get; set; }
        public int DateRangeId { get; set; }
        public DateRange DateRanges { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public ICollection<TimeRange> TimeRanges { get; set; } = new List<TimeRange>();
    }

}

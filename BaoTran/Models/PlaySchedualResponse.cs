namespace BaoTran.Models
{
    public class PlaySchedualResponse
    {
        public DayOfWeek DaysOfWeek { get; set; } //Monday, Tuesday... 
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int IdMediaFile { get; set; }
    }
}

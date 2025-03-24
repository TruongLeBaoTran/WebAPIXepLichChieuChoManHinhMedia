namespace BaoTran.Models
{
    public class PlaySchedualRequest
    {
        public string DaysOfWeek { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int IdMediaFile { get; set; }
    }
}

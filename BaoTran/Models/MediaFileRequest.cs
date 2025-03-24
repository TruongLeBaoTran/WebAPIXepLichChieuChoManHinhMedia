namespace BaoTran.Models
{
    public class MediaFileRequest
    {
        public string Title { get; set; }
        public string Singer { get; set; }
        public string Musician { get; set; }
        public string FileFormat { get; set; }
        public string Duration { get; set; }
        public IFormFile File { get; set; }
        public string FileType { get; set; }
    }
}

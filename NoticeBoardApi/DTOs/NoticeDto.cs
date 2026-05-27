namespace NoticeBoardApi.DTOs
{
    public class NoticeDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty; // formatted string
        public bool IsArchived { get; set; }
        
    }
}

using System.ComponentModel.DataAnnotations;
using com.sun.xml.@internal.ws.wsdl.writer.document.soap;

namespace NoticeBoardApi.Models
{
    public class Notice
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]

        public string Title { get; set; } = string.Empty;

        [Required]
        public string Body { get; set; } = string.Empty;
      

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsArchived { get; set; } = false;

    }
}

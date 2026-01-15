namespace EHS.Application.DTOs
{
    public class IncidentAttachmentResponse
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string AttachmentType { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UploadedByUserName { get; set; } = string.Empty;
    }
}

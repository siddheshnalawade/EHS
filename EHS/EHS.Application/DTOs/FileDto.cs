namespace EHS.Application.DTOs
{
    public class FileUploadRequest
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
    }

    public class FileUploadResponse
    {
        public string UploadUrl { get; set; } = string.Empty;
        public string UniqueFileName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Metadata sent back to the API after successful upload.
    /// </summary>
    public class AttachmentMetadataDto
    {
        public string FileName { get; set; } = string.Empty; // The unique name given by server
        public string OriginalFileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
    }
}

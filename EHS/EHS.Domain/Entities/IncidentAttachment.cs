using System;

namespace EHS.Domain.Entities
{
    public class IncidentAttachment : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid IncidentId { get; set; }
        public Incident Incident { get; set; } = null!;

        public string FileName { get; set; } = string.Empty;
        
        /// <summary>
        /// The URL or relative path to the file.
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Determine if this is initial evidence or implementation evidence.
        /// e.g. "Evidence", "Implementation", "RCA"
        /// </summary>
        public string AttachmentType { get; set; } = string.Empty; 

        public Guid UploadedByUserId { get; set; }
        public ApplicationUser UploadedByUser { get; set; } = null!;
        
        /// <summary>
        /// Original content type (MimeType) e.g. image/jpeg
        /// </summary>
        public string ContentType { get; set; } = string.Empty;

        public long FileSize { get; set; }
    }
}

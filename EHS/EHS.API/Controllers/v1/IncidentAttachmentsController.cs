using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EHS.API.Controllers.v1
{
    [ApiController]
    [Route("api/v1/incidents/attachments")]
    [Authorize]
    public class IncidentAttachmentsController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;

        public IncidentAttachmentsController(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        /// <summary>
        /// Generates a SAS Token (or local URL) for directly uploading a file.
        /// </summary>
        /// <param name="request">File metadata (name, size, type).</param>
        /// <returns>The URL to PUT the file content to, and the unique filename to save later.</returns>
        [HttpPost("generate-upload-url")]
        public async Task<IActionResult> GenerateUploadUrl([FromBody] FileUploadRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FileName))
                return BadRequest("FileName is required.");

            // 1. Validate Extension (Security)
            var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
            var allowedExtensions = new[] { ".jpg", ".  ", ".png", ".pdf", ".mp4", ".mov" }; // Added Video extensions
            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest($"File type '{extension}' is not allowed.");
            }

            // 2. Validate Size (Logic only, actual enforcement happens at Azure/Storage level or post-process)
            // Example: 100MB limit
            if (request.FileSize > 100 * 1024 * 1024)
            {
                return BadRequest("File size exceeds the limit of 100MB.");
            }

            // 3. Generate Unique Filename
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";

            // 4. Generate SAS URL
            // This URL gives Write permission to THIS specific file only.
            var uploadUrl = await _fileStorageService.GetFileUploadUrlAsync(uniqueFileName, "incidents", expiresInMinutes: 30);

            return Ok(new FileUploadResponse
            {
                UploadUrl = uploadUrl,
                UniqueFileName = uniqueFileName
            });
        }
    }
}
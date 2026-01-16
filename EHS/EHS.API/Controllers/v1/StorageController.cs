using EHS.Application.Validators.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace EHS.API.Controllers.v1
{
    /// <summary>
    /// Controller to handle file uploads when using Local Storage provider.
    /// This mimics the behavior of Azure Blob Storage PUT requests.
    /// </summary>
    [ApiController]
    [Route("api/v1/storage")]
    public class StorageController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public StorageController(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }

        /// <summary>
        /// Handles direct file upload for Local Storage provider.
        /// </summary>
        /// <param name="container">Container/Folder name</param>
        /// <param name="fileName">Target file name</param>
        /// <returns>Status of upload</returns>
        [HttpPut("upload/{container}/{fileName}")]
        [DisableRequestSizeLimit] // Allow large files (video)
        public async Task<IActionResult> UploadFile(string container, string fileName)
        {
            // 1. Basic Validation
            if (string.IsNullOrEmpty(container) || string.IsNullOrEmpty(fileName))
                return BadRequest("Invalid parameters.");

            // 2. Validate Provider is indeed Local
            var provider = _configuration["FileStorage:Provider"];
            if (!string.Equals(provider, "Local", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Direct upload is only handled by this endpoint when utilizing Local storage. For Azure, use the SAS URL.");
            }

            // 3. Prepare Path
            var basePath = _configuration["FileStorage:BasePath"] ?? "uploads";
            var folderPath = Path.Combine(_env.WebRootPath, basePath, container);
            
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = Path.Combine(folderPath, fileName);

            // 4. Stream to Disk
            // We use Request.Body directly to minimize memory usage for large files
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Request.Body.CopyToAsync(stream);
                }

                return Ok("File uploaded successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

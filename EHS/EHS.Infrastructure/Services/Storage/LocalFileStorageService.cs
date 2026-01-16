using EHS.Application.Interfaces;
using EHS.Domain.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace EHS.Infrastructure.Services.Storage
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly FileStorageSettings _settings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LocalFileStorageService(
            IWebHostEnvironment env, 
            IOptions<FileStorageSettings> settings,
            IHttpContextAccessor httpContextAccessor)
        {
            _env = env;
            _settings = settings.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string containerName)
        {
            var (isValid, errorMessage) = await EHS.Application.Validators.Utilities.FileValidator.ValidateFileAsync(file);
            if (!isValid)
            {
                throw new ArgumentException($"File validation failed: {errorMessage}");
            }

            // Create Directory: wwwroot/uploads/incidents
            var folderPath = Path.Combine(_env.WebRootPath, _settings.BasePath, containerName);
            
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Generate unique filename
            var extension = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(folderPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return ONLY filename
            return uniqueFileName;
        }

        public Task DeleteFileAsync(string fileName, string containerName)
        {
            try
            {
                var filePath = Path.Combine(_env.WebRootPath, _settings.BasePath, containerName, fileName);
                
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch
            {
                // Log warning
            }
            return Task.CompletedTask;
        }

        public Task<string> GetFileUrlAsync(string fileName, string containerName, int expiresInMinutes = 60)
        {
            // Local storage doesn't support SAS, so we return the static URL.
            // In a real scenario, you could implement a "Proxy Controller" that checks permissions before serving files.
            
            var request = _httpContextAccessor.HttpContext?.Request;
            var baseUrl = $"{request?.Scheme}://{request?.Host}";
            
            var url = $"{baseUrl}/{_settings.BasePath}/{containerName}/{fileName}";
            return Task.FromResult(url);
        }

        public Task<string> GetFileUploadUrlAsync(string fileName, string containerName, int expiresInMinutes = 15)
        {
            // For Local storage, we return a URL to our own API controller that handles the stream.
            // e.g. /api/v1/storage/upload/incidents/guid.jpg
            
            var request = _httpContextAccessor.HttpContext?.Request;
            var baseUrl = $"{request?.Scheme}://{request?.Host}";
            
            var url = $"{baseUrl}/api/v1/storage/upload/{containerName}/{fileName}";
            return Task.FromResult(url);
        }
    }
}

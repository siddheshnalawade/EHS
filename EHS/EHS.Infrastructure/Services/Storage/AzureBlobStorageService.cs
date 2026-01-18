using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using EHS.Application.Interfaces;
using EHS.Domain.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace EHS.Infrastructure.Services.Storage
{
    public class AzureBlobStorageService : IFileStorageService
    {
        private readonly FileStorageSettings _settings;
        private readonly BlobServiceClient _blobServiceClient;

        public AzureBlobStorageService(IOptions<FileStorageSettings> settings)
        {
            _settings = settings.Value;
            _blobServiceClient = new BlobServiceClient(_settings.ConnectionString);
        }

        public async Task<string> UploadFileAsync(IFormFile file, string containerName)
        {
            var (isValid, errorMessage) = await EHS.Application.Validators.Utilities.FileValidator.ValidateFileAsync(file);
            if (!isValid)
            {
                throw new ArgumentException($"File validation failed: {errorMessage}");
            }

            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
            
            // Set access policy to Private (No public access)
            // await containerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.None); 
            // Warning: Be careful changing existing container policies in prod.
            // For new containers it defaults to private. We will explicitly NOT set it to Blob.

            var extension = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var blobClient = containerClient.GetBlobClient(uniqueFileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            // Return only the filename, NOT the full URL.
            // The full URL will be generated dynamically with SAS token.
            return uniqueFileName; 
        }

        public async Task DeleteFileAsync(string fileName, string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            
            await blobClient.DeleteIfExistsAsync();
        }

        public async Task<string> GetFileUrlAsync(string fileName, string containerName, int expiresInMinutes = 60)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            if (!blobClient.CanGenerateSasUri)
            {
                return string.Empty;
            }

            var sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = containerName,
                BlobName = fileName,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5), // Allow for clock skew
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expiresInMinutes),
                Protocol = SasProtocol.HttpsAndHttp // Dev usually Http
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sasUri = blobClient.GenerateSasUri(sasBuilder);
            return sasUri.ToString();
        }

        public async Task<string> GetFileUploadUrlAsync(string fileName, string containerName, int expiresInMinutes = 15)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            
            // Ensure container exists before we give out a URL to upload to it
            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient(fileName);

            if (!blobClient.CanGenerateSasUri)
            {
                return string.Empty;
            }

            var sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = containerName,
                BlobName = fileName,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expiresInMinutes),
                Protocol = SasProtocol.HttpsAndHttp
            };

            // Grant Write/Create permissions for upload
            sasBuilder.SetPermissions(BlobSasPermissions.Write | BlobSasPermissions.Create);

            var sasUri = blobClient.GenerateSasUri(sasBuilder);
            return sasUri.ToString();
        }
    }
}

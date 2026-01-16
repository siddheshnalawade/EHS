using Microsoft.AspNetCore.Http;

namespace EHS.Application.Interfaces
{
    public interface IFileStorageService
    {
        /// <summary>
        /// Uploads a file to the configured storage provider.
        /// </summary>
        /// <param name="file">The file stream and metadata.</param>
        /// <param name="containerName">The container or folder name (e.g. "incidents", "profiles").</param>
        /// <returns>The unique URL or Path to access the file.</returns>
        Task<string> UploadFileAsync(IFormFile file, string containerName);

        /// <summary>
        /// Deletes a file from the storage provider.
        /// </summary>
        Task DeleteFileAsync(string fileRoute, string containerName);

        /// <summary>
        /// Generates a pre-signed URL for uploading a file directly to the storage.
        /// </summary>
        /// <param name="fileName">The unique file name.</param>
        /// <param name="containerName">The container name.</param>
        /// <param name="expiresInMinutes">Time until expiration.</param>
        /// <returns>The URL to PUT the file content to.</returns>
        Task<string> GetFileUploadUrlAsync(string fileName, string containerName, int expiresInMinutes = 15);

        /// <summary>
        /// Generates a secure, temporary URL for accessing the file.
        /// </summary>
        /// <param name="fileName">The name of the file in storage.</param>
        /// <param name="containerName">The container name.</param>
        /// <param name="expiresInMinutes">How long the link is valid.</param>
        /// <returns>A signed URL or public URL depending on implementation.</returns>
        Task<string> GetFileUrlAsync(string fileName, string containerName, int expiresInMinutes = 60);
    }
}

using Microsoft.AspNetCore.Http;

namespace EHS.Application.Validators.Utilities
{
    public static class FileValidator
    {
        private const int MaxFileSizeInBytes = 10 * 1024 * 1024; // 10 MB
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };

        // Magic numbers for file signatures
        private static readonly Dictionary<string, List<byte[]>> FileSignatures = new()
        {
            { ".jpg", new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF } } },
            { ".jpeg", new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF } } },
            { ".png", new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } },
            { ".pdf", new List<byte[]> { new byte[] { 0x25, 0x50, 0x44, 0x46 } } }
        };

        public static async Task<(bool IsValid, string ErrorMessage)> ValidateFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return (false, "File is empty.");
            }

            if (file.Length > MaxFileSizeInBytes)
            {
                return (false, $"File size exceeds the limit of {MaxFileSizeInBytes / 1024 / 1024} MB.");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                return (false, "File type not allowed. Please upload JPG, PNG, or PDF.");
            }

            // Read first few bytes to verify signature
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();

                if (FileSignatures.TryGetValue(extension, out var signatures))
                {
                    var isMatch = signatures.Any(signature => 
                        fileBytes.Take(signature.Length).SequenceEqual(signature));

                    if (!isMatch)
                    {
                        return (false, "Invalid file content (Signature mismatch).");
                    }
                }
            }

            // Reset stream position if needed, though IFormFile.OpenReadStream() returns a new stream usually
            return (true, string.Empty);
        }
    }
}

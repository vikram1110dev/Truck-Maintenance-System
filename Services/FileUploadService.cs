using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Truck_Maintanance_system.Services
{
    public interface IFileUploadService
    {
        Task<string?> SaveFileAsync(IFormFile file, string subFolder, string? customFileName = null);
        void DeleteFile(string relativePath);
    }

    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _env;
        private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp"
        };
        private static readonly HashSet<string> AllowedDocumentExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf", ".jpg", ".jpeg", ".png", ".gif", ".webp", ".doc", ".docx"
        };
        private static readonly HashSet<string> AllowedAudioExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".webm", ".m4a", ".mp3", ".ogg", ".wav"
        };

        private const long MaxImageSizeBytes = 5 * 1024 * 1024;      // 5 MB
        private const long MaxDocumentSizeBytes = 10 * 1024 * 1024;   // 10 MB
        private const long MaxAudioSizeBytes = 10 * 1024 * 1024;      // 10 MB

        public FileUploadService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string?> SaveFileAsync(IFormFile file, string subFolder, string? customFileName = null)
        {
            if (file == null || file.Length == 0)
                return null;

            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            // Validate file type and size
            if (AllowedImageExtensions.Contains(extension))
            {
                if (file.Length > MaxImageSizeBytes)
                    throw new InvalidOperationException($"Image file exceeds maximum size of {MaxImageSizeBytes / (1024 * 1024)} MB.");
            }
            else if (AllowedDocumentExtensions.Contains(extension))
            {
                if (file.Length > MaxDocumentSizeBytes)
                    throw new InvalidOperationException($"Document file exceeds maximum size of {MaxDocumentSizeBytes / (1024 * 1024)} MB.");
            }
            else if (AllowedAudioExtensions.Contains(extension))
            {
                if (file.Length > MaxAudioSizeBytes)
                    throw new InvalidOperationException($"Audio file exceeds maximum size of {MaxAudioSizeBytes / (1024 * 1024)} MB.");
            }
            else
            {
                throw new InvalidOperationException($"File type '{extension}' is not allowed.");
            }

            string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", subFolder);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = customFileName ?? (Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName));
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{subFolder}/{uniqueFileName}";
        }

        public void DeleteFile(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return;

            string filePath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}

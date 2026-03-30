using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace learning.Services
{
    public interface IImageService
    {
        Task<string?> UploadImageAsync(IFormFile file, string subfolder);
        Task DeleteImageAsync(string relativePath);
    }

    public class LocalImageService : IImageService
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<LocalImageService> _logger;

        public LocalImageService(IWebHostEnvironment env, ILogger<LocalImageService> logger)
        {
            _env = env;
            _logger = logger;
        }

        // Max 5MB per image
        private const long MaxBytes = 5 * 1024 * 1024;
        private static readonly string[] AllowedTypes = { "image/jpeg", "image/png", "image/webp" };

        

        public async Task<string?> UploadImageAsync(IFormFile file, string subfolder)
        {
            // Validate type and size
            if (!AllowedTypes.Contains(file.ContentType))
            {
                _logger.LogWarning("Rejected upload: invalid type {Type}", file.ContentType);
                return null;
            }
            if (file.Length > MaxBytes)
            {
                _logger.LogWarning("Rejected upload: file too large ({Size} bytes)", file.Length);
                return null;
            }

            // Build safe file path — never trust the original filename
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var safeFile = $"{Guid.NewGuid()}{ext}";
            var folder = Path.Combine(_env.WebRootPath, "uploads", subfolder);
            Directory.CreateDirectory(folder); // no-op if already exists

            var fullPath = Path.Combine(folder, safeFile);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            // Return the relative URL used in <img src="...">
            return $"/uploads/{subfolder}/{safeFile}";
        }

        public Task DeleteImageAsync(string relativePath)
        {
            try
            {
                // relativePath = "/uploads/products/abc.jpg"
                var fullPath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/'));
                if (File.Exists(fullPath))
                    File.Delete(fullPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete image at {Path}", relativePath);
            }
            return Task.CompletedTask;
        }


    }
}

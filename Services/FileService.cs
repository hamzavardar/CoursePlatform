namespace CoursePlatform.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _env;

    public FileService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<(bool IsValid, string? FilePath, string? ErrorMessage)> UploadImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return (false, null, "Geçersiz dosya.");

        if (file.Length > 5 * 1024 * 1024)
            return (false, null, "Görsel boyutu en fazla 5MB olabilir.");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(ext))
            return (false, null, "Sadece JPG, PNG veya WEBP yükleyebilirsiniz.");

        using var stream = file.OpenReadStream();
        var header = new byte[8];
        await stream.ReadAsync(header, 0, header.Length);

        if (!IsImageMagicBytesValid(header, ext))
            return (false, null, "Geçersiz görsel içeriği.");

        string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "covers");
        Directory.CreateDirectory(uploadsFolder);

        string uniqueFileName = $"{Guid.NewGuid():N}{ext}";
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return (true, "/uploads/covers/" + uniqueFileName, null);
    }

    public async Task<(bool IsValid, string? FilePath, string? ErrorMessage)> UploadVideoAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return (false, null, "Geçersiz video dosyası.");

        if (file.Length > 500 * 1024 * 1024)
            return (false, null, "Video boyutu en fazla 500MB olabilir.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (ext != ".mp4")
            return (false, null, "Yalnızca .mp4 videoları kabul edilir.");

        using var stream = file.OpenReadStream();
        var header = new byte[12];
        await stream.ReadAsync(header, 0, header.Length);

        if (!IsMp4MagicBytesValid(header))
            return (false, null, "Bozuk veya zararlı video içeriği.");

        string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "videos");
        Directory.CreateDirectory(uploadsFolder);

        string uniqueFileName = $"{Guid.NewGuid():N}{ext}";
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return (true, "/uploads/videos/" + uniqueFileName, null);
    }

    public void DeleteFile(string relativePath)
    {
        if (string.IsNullOrEmpty(relativePath)) return;

        string fullPath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(fullPath))
        {
            try { File.Delete(fullPath); } catch { }
        }
    }

    private static bool IsImageMagicBytesValid(byte[] header, string extension)
    {
        return extension switch
        {
            ".jpg" or ".jpeg" => header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
            ".png" => header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47,
            ".webp" => header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46,
            _ => false
        };
    }

    private static bool IsMp4MagicBytesValid(byte[] header)
    {
        return header.Length >= 8 && header[4] == 0x66 && header[5] == 0x74 && header[6] == 0x79 && header[7] == 0x70;
    }
}
namespace CoursePlatform.Services;

public interface IFileService
{
    Task<(bool IsValid, string? FilePath, string? ErrorMessage)> UploadImageAsync(IFormFile file);
    Task<(bool IsValid, string? FilePath, string? ErrorMessage)> UploadVideoAsync(IFormFile file);
    void DeleteFile(string relativePath);
}
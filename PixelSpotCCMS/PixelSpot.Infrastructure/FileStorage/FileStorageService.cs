using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace PixelSpot.Infrastructure.FileStorage;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(string base64Content, string fileName, string contentType);
    Task DeleteFileAsync(string fileUrl);
    string GetFileUrl(string fileName);
}

public class FileStorageService : IFileStorageService
{
    private readonly FileStorageSettings _settings;
    private readonly ILogger<FileStorageService> _logger;

    public FileStorageService(IOptions<FileStorageSettings> settings, ILogger<FileStorageService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<string> SaveFileAsync(string base64Content, string fileName, string contentType)
    {
        try
        {
            // Remove the data:image/png;base64, part if present
            var base64Data = Regex.Replace(base64Content, @"^data:image\/[a-zA-Z]+;base64,", string.Empty);
            
            // Create a unique file name
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var filePath = Path.Combine(_settings.StoragePath, uniqueFileName);
            
            // Create directory if it doesn't exist
            Directory.CreateDirectory(_settings.StoragePath);
            
            // Save the file
            await File.WriteAllBytesAsync(filePath, Convert.FromBase64String(base64Data));
            
            // Return the URL
            return GetFileUrl(uniqueFileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving file {FileName}", fileName);
            throw;
        }
    }

    public async Task DeleteFileAsync(string fileUrl)
    {
        try
        {
            var fileName = Path.GetFileName(new Uri(fileUrl).AbsolutePath);
            var filePath = Path.Combine(_settings.StoragePath, fileName);
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {FileUrl}", fileUrl);
            throw;
        }
    }

    public string GetFileUrl(string fileName)
    {
        return $"{_settings.BaseUrl}/{fileName}";
    }
}

public class FileStorageSettings
{
    public string StoragePath { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
}

using Microsoft.Extensions.Configuration;
using PhotoOrganizer.Application.Abstractions;

namespace PhotoOrganizer.Infrastructure.FileStorage;

public class LocalFileStorage(IConfiguration configuration) : ILocalFileStorage
{
    private static readonly HashSet<string> SupportedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tif", ".tiff", ".webp", ".heic"
    };

    private readonly string _rootPath = Path.GetFullPath(
        configuration["PhotoStorage:RootPath"]
        ?? throw new InvalidOperationException("PhotoStorage:RootPath must be configured."));

    public IReadOnlyList<string> EnumerateImages()
    {
        if (!Directory.Exists(_rootPath))
            return Array.Empty<string>();

        return Directory.EnumerateFiles(_rootPath, "*", SearchOption.AllDirectories)
            .Where(path => SupportedExtensions.Contains(Path.GetExtension(path)))
            .ToArray();
    }

    public bool IsInsideLibrary(string path)
    {
        var fullPath = Path.GetFullPath(path);
        var relativePath = Path.GetRelativePath(_rootPath, fullPath);
        return !Path.IsPathRooted(relativePath)
            && relativePath != ".."
            && !relativePath.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal);
    }
}

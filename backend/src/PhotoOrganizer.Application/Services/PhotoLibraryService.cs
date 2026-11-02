using PhotoOrganizer.Application.Abstractions;
using PhotoOrganizer.Domain;
using Microsoft.Extensions.Logging;

namespace PhotoOrganizer.Application.Services;

public class PhotoLibraryService(
    IAlbumRepository repository,
    ILocalFileStorage fileStorage,
    IPhotoMetadataReader metadataReader,
    ILogger<PhotoLibraryService> logger,
    AlbumGroupingService groupingService)
{
    public async Task<int> ImportLocalPhotosAsync(CancellationToken cancellationToken = default)
    {
        var existingPhotos = await repository.GetPhotosAsync(cancellationToken);
        var allPhotos = existingPhotos.ToList();
        var existingByPath = existingPhotos.ToDictionary(
            photo => Path.GetFullPath(photo.OriginalPath),
            StringComparer.OrdinalIgnoreCase);
        var changedPhotos = new List<Photo>();
        var importedCount = 0;
        foreach (var path in fileStorage.EnumerateImages())
        {
            var fullPath = Path.GetFullPath(path);
            var fileInfo = new FileInfo(fullPath);
            if (!fileInfo.Exists)
            {
                logger.LogWarning("Photo file disappeared before it could be imported: {PhotoPath}.", fullPath);
                continue;
            }

            var modifiedAtUtc = fileInfo.LastWriteTimeUtc;
            if (existingByPath.TryGetValue(fullPath, out var existing))
            {
                if (existing.FileSizeBytes == fileInfo.Length && existing.FileModifiedAtUtc == modifiedAtUtc)
                    continue;

                existing.FileSizeBytes = fileInfo.Length;
                existing.FileModifiedAtUtc = modifiedAtUtc;
                existing.CaptureDate = metadataReader.ReadCaptureDateUtc(fullPath);
                existing.UpdatedAtUtc = DateTime.UtcNow;
                changedPhotos.Add(existing);
                continue;
            }

            var photo = new Photo
            {
                OriginalPath = fullPath,
                FileName = Path.GetFileName(fullPath),
                CaptureDate = metadataReader.ReadCaptureDateUtc(fullPath),
                FileSizeBytes = fileInfo.Length,
                FileModifiedAtUtc = modifiedAtUtc,
                ContentType = GetContentType(Path.GetExtension(fullPath))
            };
            existingByPath.Add(fullPath, photo);
            allPhotos.Add(photo);
            changedPhotos.Add(photo);
            importedCount++;
        }

        if (changedPhotos.Count > 0)
            await repository.SavePhotosAsync(changedPhotos, cancellationToken);

        var groups = groupingService.GroupPhotosByDate(allPhotos);
        await repository.SynchronizeDateGroupsAsync(groups, cancellationToken);
        return importedCount;
    }

    private static string? GetContentType(string extension) => extension.ToLowerInvariant() switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".gif" => "image/gif",
        ".bmp" => "image/bmp",
        ".tif" or ".tiff" => "image/tiff",
        ".webp" => "image/webp",
        ".heic" => "image/heic",
        _ => null
    };
}

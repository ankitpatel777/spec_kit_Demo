using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Microsoft.Extensions.Logging;
using PhotoOrganizer.Application.Abstractions;

namespace PhotoOrganizer.Infrastructure.FileStorage;

public class PhotoMetadataReader(ILogger<PhotoMetadataReader> logger) : IPhotoMetadataReader
{
    public DateTime? ReadCaptureDateUtc(string filePath)
    {
        try
        {
            var directories = ImageMetadataReader.ReadMetadata(filePath);
            var exif = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
            if (exif is null)
                return null;

            if (exif.TryGetDateTime(ExifSubIfdDirectory.TagDateTimeOriginal, out var captureDate)
                || exif.TryGetDateTime(ExifSubIfdDirectory.TagDateTimeDigitized, out captureDate))
            {
                return DateTime.SpecifyKind(captureDate, DateTimeKind.Utc);
            }
        }
        catch (ImageProcessingException exception)
        {
            logger.LogWarning(exception, "Could not read photo metadata from {PhotoPath}.", filePath);
        }
        catch (IOException exception)
        {
            logger.LogWarning(exception, "Could not access photo metadata from {PhotoPath}.", filePath);
        }
        catch (UnauthorizedAccessException exception)
        {
            logger.LogWarning(exception, "Access was denied while reading photo metadata from {PhotoPath}.", filePath);
        }

        return null;
    }
}

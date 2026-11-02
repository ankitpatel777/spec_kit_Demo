using Microsoft.Extensions.Logging.Abstractions;
using PhotoOrganizer.Infrastructure.FileStorage;

namespace PhotoOrganizer.Application.Tests;

public class PhotoMetadataReaderTests
{
    [Fact]
    public async Task ReadCaptureDateUtc_ReturnsUnresolvedForInvalidImageMetadata()
    {
        var imagePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.jpg");
        await File.WriteAllBytesAsync(imagePath, new byte[] { 1, 2, 3 });
        try
        {
            var reader = new PhotoMetadataReader(NullLogger<PhotoMetadataReader>.Instance);

            Assert.Null(reader.ReadCaptureDateUtc(imagePath));
        }
        finally
        {
            File.Delete(imagePath);
        }
    }
}

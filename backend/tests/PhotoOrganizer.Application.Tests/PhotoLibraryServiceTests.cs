using Microsoft.Extensions.Logging.Abstractions;
using PhotoOrganizer.Application.Abstractions;
using PhotoOrganizer.Application.Services;

namespace PhotoOrganizer.Application.Tests;

public class PhotoLibraryServiceTests
{
    [Fact]
    public async Task ImportLocalPhotosAsync_PersistsLocalFilesAndUnresolvedDateGroup()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"photo-organizer-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        var imagePath = Path.Combine(directory, "unknown.jpg");
        await File.WriteAllBytesAsync(imagePath, new byte[] { 1, 2, 3 });
        try
        {
            var repository = new TestAlbumRepository();
            var service = CreateService(repository, imagePath, new TestMetadataReader());

            var imported = await service.ImportLocalPhotosAsync();
            var photos = await repository.GetPhotosAsync();
            var albums = await repository.GetAlbumsAsync();

            Assert.Equal(1, imported);
            Assert.Equal(Path.GetFullPath(imagePath), Assert.Single(photos).OriginalPath);
            Assert.False(photos.Single().IsDateResolved);
            Assert.Equal("Unspecified Dates", Assert.Single(albums).Name);
            Assert.Single(albums.Single().AlbumPhotos);
            Assert.Equal(0, await service.ImportLocalPhotosAsync());
            Assert.Single(await repository.GetPhotosAsync());
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public async Task ImportLocalPhotosAsync_RefreshesCaptureDateWhenFileChanges()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"photo-organizer-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        var imagePath = Path.Combine(directory, "photo.jpg");
        await File.WriteAllBytesAsync(imagePath, new byte[] { 1 });
        try
        {
            var repository = new TestAlbumRepository();
            var metadataReader = new TestMetadataReader();
            var service = CreateService(repository, imagePath, metadataReader);
            await service.ImportLocalPhotosAsync();

            var captureDate = new DateTime(2026, 6, 13, 10, 0, 0, DateTimeKind.Utc);
            metadataReader.CaptureDate = captureDate;
            await File.WriteAllBytesAsync(imagePath, new byte[] { 1, 2, 3 });
            File.SetLastWriteTimeUtc(imagePath, DateTime.UtcNow.AddMinutes(-1));
            await service.ImportLocalPhotosAsync();

            Assert.True(Assert.Single(await repository.GetPhotosAsync()).IsDateResolved);
            Assert.Equal("2026-06-13", Assert.Single(await repository.GetAlbumsAsync()).Name);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private static PhotoLibraryService CreateService(
        TestAlbumRepository repository,
        string imagePath,
        TestMetadataReader metadataReader) =>
        new(
            repository,
            new TestFileStorage(imagePath),
            metadataReader,
            NullLogger<PhotoLibraryService>.Instance,
            new AlbumGroupingService());

    private sealed class TestFileStorage(string path) : ILocalFileStorage
    {
        public IReadOnlyList<string> EnumerateImages() => new[] { path };
        public bool IsInsideLibrary(string filePath) => Path.GetFullPath(filePath) == Path.GetFullPath(path);
    }

    private sealed class TestMetadataReader : IPhotoMetadataReader
    {
        public DateTime? CaptureDate { get; set; }
        public DateTime? ReadCaptureDateUtc(string filePath) => CaptureDate;
    }
}

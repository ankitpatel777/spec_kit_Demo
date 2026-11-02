using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using PhotoOrganizer.Api.Controllers;
using PhotoOrganizer.Application.Abstractions;
using PhotoOrganizer.Application.DTOs;
using PhotoOrganizer.Application.Services;
using PhotoOrganizer.Infrastructure.Data;
using PhotoOrganizer.Infrastructure.Repositories;

namespace PhotoOrganizer.Application.Tests;

public class AlbumWorkflowIntegrationTests
{
    [Fact]
    public async Task ImportBrowseCreateAndRename_PreservesDateGroupsAndTileMetadata()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"photo-organizer-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        var imagePath = Path.Combine(directory, "trip.jpg");
        await File.WriteAllBytesAsync(imagePath, new byte[] { 1, 2, 3 });
        var options = new DbContextOptionsBuilder<PhotoOrganizerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        try
        {
            await using var context = new PhotoOrganizerDbContext(options);
            var repository = new AlbumRepository(context);
            var groupingService = new AlbumGroupingService();
            var fileStorage = new TestFileStorage(imagePath);
            var libraryService = new PhotoLibraryService(
                repository,
                fileStorage,
                new TestMetadataReader(new DateTime(2026, 6, 13, 10, 0, 0, DateTimeKind.Utc)),
                NullLogger<PhotoLibraryService>.Instance,
                groupingService);
            var photoController = new PhotoController(repository, fileStorage, libraryService);
            var albumController = new AlbumController(
                repository,
                new AlbumManagementService(repository),
                new AlbumSummaryService());

            var importResult = await photoController.ImportPhotos(CancellationToken.None);
            Assert.IsType<OkObjectResult>(importResult.Result);

            var albumsResult = await albumController.GetAlbums(CancellationToken.None);
            var albums = Assert.IsType<OkObjectResult>(albumsResult.Result).Value as IEnumerable<AlbumSummaryDto>;
            var dateAlbum = Assert.Single(albums!);
            Assert.Equal("2026-06-13", dateAlbum.Name);
            Assert.Equal(1, dateAlbum.PhotoCount);
            Assert.Equal($"/api/photos/{dateAlbum.CoverPhotoId}/content", dateAlbum.CoverImageUrl);

            var detailsResult = await albumController.GetAlbum(dateAlbum.Id, CancellationToken.None);
            var details = Assert.IsType<OkObjectResult>(detailsResult.Result).Value as AlbumDto;
            Assert.True(details!.Photos.Single().IsDateResolved);
            var photoListResult = await albumController.GetAlbumPhotos(dateAlbum.Id, CancellationToken.None);
            var photoList = Assert.IsAssignableFrom<IEnumerable<PhotoDto>>(
                Assert.IsType<OkObjectResult>(photoListResult.Result).Value);
            Assert.Single(photoList);

            var createdResult = await albumController.CreateAlbum(
                new CreateAlbumRequest { Name = "Weekend trip" },
                CancellationToken.None);
            var created = Assert.IsType<CreatedAtActionResult>(createdResult.Result).Value as AlbumDto;
            var updatedResult = await albumController.UpdateAlbum(
                created!.Id,
                new UpdateAlbumRequest { Name = "Renamed trip" },
                CancellationToken.None);

            var updated = Assert.IsType<OkObjectResult>(updatedResult.Result).Value as AlbumDto;
            Assert.Equal("Renamed trip", updated?.Name);
            await albumController.UpdateAlbum(
                dateAlbum.Id,
                new UpdateAlbumRequest { Name = "Renamed date group" },
                CancellationToken.None);
            await photoController.ImportPhotos(CancellationToken.None);
            var refreshedResult = await albumController.GetAlbums(CancellationToken.None);
            var refreshed = Assert.IsAssignableFrom<IEnumerable<AlbumSummaryDto>>(
                Assert.IsType<OkObjectResult>(refreshedResult.Result).Value);
            Assert.Contains(refreshed, album => album.Name == "Renamed date group");
            Assert.Contains(refreshed, album => album.Name == "Renamed trip");
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private sealed class TestFileStorage(string imagePath) : ILocalFileStorage
    {
        public IReadOnlyList<string> EnumerateImages() => new[] { imagePath };
        public bool IsInsideLibrary(string path) => Path.GetFullPath(path) == Path.GetFullPath(imagePath);
    }

    private sealed class TestMetadataReader(DateTime captureDate) : IPhotoMetadataReader
    {
        public DateTime? ReadCaptureDateUtc(string filePath) => captureDate;
    }
}

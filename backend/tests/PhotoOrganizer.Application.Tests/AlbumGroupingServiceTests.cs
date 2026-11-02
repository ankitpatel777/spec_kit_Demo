using System.Diagnostics;
using PhotoOrganizer.Application.Services;
using PhotoOrganizer.Domain;

namespace PhotoOrganizer.Application.Tests;

public class AlbumGroupingServiceTests
{
    [Fact]
    public void GroupPhotosByDate_SortsGroupsAndKeepsUnresolvedPhotosTogether()
    {
        var service = new AlbumGroupingService();
        var photos = new[]
        {
            new Photo { Id = Guid.NewGuid(), FileName = "later.jpg", CaptureDate = new DateTime(2026, 11, 2), OriginalPath = "later.jpg" },
            new Photo { Id = Guid.NewGuid(), FileName = "same-b.jpg", CaptureDate = new DateTime(2026, 11, 1), OriginalPath = "same-b.jpg" },
            new Photo { Id = Guid.NewGuid(), FileName = "same-a.jpg", CaptureDate = new DateTime(2026, 11, 1), OriginalPath = "same-a.jpg" },
            new Photo { Id = Guid.NewGuid(), FileName = "unknown.jpg", OriginalPath = "unknown.jpg" }
        };

        var result = service.GroupPhotosByDate(photos);

        Assert.Equal(new[] { "2026-11-01", "2026-11-02", "Unspecified Dates" }, result.Select(album => album.Name));
        Assert.Equal(2, result[0].AlbumPhotos.Count);
        Assert.Equal(2, result[0].AlbumPhotos.Select(membership => membership.PhotoId).Distinct().Count());
        Assert.False(result[^1].Photos.Single().IsDateResolved);
    }

    [Fact]
    public void CreateSummary_UsesCoverAndReportsPhotoCountAndDateRange()
    {
        var photo = new Photo
        {
            Id = Guid.NewGuid(),
            FileName = "cover.jpg",
            OriginalPath = "cover.jpg",
            CaptureDate = new DateTime(2026, 11, 1)
        };
        var album = new Album
        {
            Id = Guid.NewGuid(),
            Name = "2026-11-01",
            StartDate = photo.CaptureDate,
            EndDate = photo.CaptureDate,
            AlbumPhotos = new List<AlbumPhoto>
            {
                new() { Photo = photo, PhotoId = photo.Id, AlbumId = Guid.NewGuid(), SortOrder = 0 }
            }
        };

        var summary = new AlbumSummaryService().CreateSummary(album);

        Assert.Equal("2026-11-01", summary.DateRange);
        Assert.Equal(1, summary.PhotoCount);
        Assert.Equal(photo.Id, summary.CoverPhotoId);
        Assert.Equal($"/api/photos/{photo.Id}/content", summary.CoverImageUrl);
        Assert.False(summary.IsEmpty);
    }

    [Fact]
    public void CreateSummary_ReportsEmptyAlbumsClearly()
    {
        var summary = new AlbumSummaryService().CreateSummary(new Album { Name = "Empty" });

        Assert.True(summary.IsEmpty);
        Assert.Equal(0, summary.PhotoCount);
        Assert.Null(summary.CoverImageUrl);
        Assert.Equal("Unspecified dates", summary.DateRange);
    }

    [Fact]
    public void GroupPhotosByDate_HandlesThousandsOfPhotosWithinTwoSeconds()
    {
        var photos = Enumerable.Range(0, 10_000)
            .Select(index => new Photo
            {
                Id = Guid.NewGuid(),
                FileName = $"{index:D5}.jpg",
                OriginalPath = $"C:\\Photos\\{index:D5}.jpg",
                CaptureDate = new DateTime(2026, 1, 1).AddDays(index % 365)
            })
            .ToArray();
        var service = new AlbumGroupingService();
        var stopwatch = Stopwatch.StartNew();

        var groups = service.GroupPhotosByDate(photos);

        stopwatch.Stop();
        Assert.Equal(365, groups.Count);
        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(2), $"Grouping took {stopwatch.Elapsed}.");
    }
}

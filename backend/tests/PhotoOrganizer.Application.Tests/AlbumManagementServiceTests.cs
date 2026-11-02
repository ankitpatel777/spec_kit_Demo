using PhotoOrganizer.Application.Abstractions;
using PhotoOrganizer.Application.DTOs;
using PhotoOrganizer.Application.Services;
using PhotoOrganizer.Domain;

namespace PhotoOrganizer.Application.Tests;

public class AlbumManagementServiceTests
{
    [Fact]
    public async Task CreateAlbumAsync_PersistsTrimmedNameAndDateRange()
    {
        var repository = new TestAlbumRepository();
        var service = new AlbumManagementService(repository);

        var album = await service.CreateAlbumAsync(new CreateAlbumRequest
        {
            Name = "  Summer trip  ",
            StartDateUtc = new DateTimeOffset(2026, 6, 1, 0, 0, 0, TimeSpan.Zero),
            EndDateUtc = new DateTimeOffset(2026, 6, 3, 0, 0, 0, TimeSpan.Zero)
        });

        Assert.Equal("Summer trip", album.Name);
        Assert.Same(album, await repository.GetAlbumAsync(album.Id));
    }

    [Fact]
    public async Task CreateAlbumAsync_RejectsBlankNameAndReversedDateRange()
    {
        var service = new AlbumManagementService(new TestAlbumRepository());

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAlbumAsync(new CreateAlbumRequest
        {
            Name = " "
        }));
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAlbumAsync(new CreateAlbumRequest
        {
            Name = "Invalid dates",
            StartDateUtc = new DateTimeOffset(2026, 6, 4, 0, 0, 0, TimeSpan.Zero),
            EndDateUtc = new DateTimeOffset(2026, 6, 1, 0, 0, 0, TimeSpan.Zero)
        }));
    }

    [Fact]
    public async Task UpdateAlbumAsync_RenamesWithoutChangingMembership()
    {
        var repository = new TestAlbumRepository();
        var photo = new Photo { Id = Guid.NewGuid(), FileName = "photo.jpg", OriginalPath = "photo.jpg" };
        var album = new Album
        {
            Name = "Before",
            AlbumPhotos = new List<AlbumPhoto>
            {
                new() { AlbumId = Guid.NewGuid(), PhotoId = photo.Id, Photo = photo }
            }
        };
        await repository.SaveAlbumAsync(album);
        var service = new AlbumManagementService(repository);

        var updated = await service.UpdateAlbumAsync(album.Id, new UpdateAlbumRequest { Name = "After" });

        Assert.Equal("After", updated?.Name);
        Assert.Single(updated!.AlbumPhotos);
        Assert.Equal(photo.Id, updated.AlbumPhotos.Single().PhotoId);
    }

    [Fact]
    public async Task UpdateAlbumAsync_RejectsDateRangeInvalidatedByOneSidedPatch()
    {
        var repository = new TestAlbumRepository();
        var album = new Album
        {
            Name = "Trip",
            StartDate = new DateTime(2026, 6, 1),
            EndDate = new DateTime(2026, 6, 3)
        };
        await repository.SaveAlbumAsync(album);
        var service = new AlbumManagementService(repository);

        await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateAlbumAsync(
            album.Id,
            new UpdateAlbumRequest { StartDateUtc = new DateTimeOffset(2026, 6, 4, 0, 0, 0, TimeSpan.Zero) }));
    }
}

internal sealed class TestAlbumRepository : IAlbumRepository
{
    private readonly List<Album> _albums = new();
    private readonly List<Photo> _photos = new();

    public Task<IReadOnlyList<Album>> GetAlbumsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Album>>(_albums);

    public Task<Album?> GetAlbumAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_albums.FirstOrDefault(album => album.Id == id));

    public Task<Photo?> GetPhotoAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_photos.FirstOrDefault(photo => photo.Id == id)
            ?? _albums.SelectMany(album => album.AlbumPhotos).Select(membership => membership.Photo).FirstOrDefault(photo => photo.Id == id));

    public Task<IReadOnlyList<Photo>> GetPhotosAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Photo>>(_photos);

    public Task<Album> SaveAlbumAsync(Album album, CancellationToken cancellationToken = default)
    {
        var existing = _albums.FindIndex(item => item.Id == album.Id);
        if (existing < 0)
            _albums.Add(album);
        else
            _albums[existing] = album;
        return Task.FromResult(album);
    }

    public Task SavePhotosAsync(IReadOnlyCollection<Photo> photos, CancellationToken cancellationToken = default)
    {
        foreach (var photo in photos)
        {
            var index = _photos.FindIndex(existing => existing.Id == photo.Id);
            if (index < 0)
                _photos.Add(photo);
            else
                _photos[index] = photo;
        }
        return Task.CompletedTask;
    }

    public Task SynchronizeDateGroupsAsync(IReadOnlyCollection<Album> groups, CancellationToken cancellationToken = default)
    {
        _albums.RemoveAll(album => album.IsDateGroup);
        _albums.AddRange(groups);
        return Task.CompletedTask;
    }
}

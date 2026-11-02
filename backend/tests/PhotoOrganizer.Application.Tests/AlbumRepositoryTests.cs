using Microsoft.EntityFrameworkCore;
using PhotoOrganizer.Domain;
using PhotoOrganizer.Infrastructure.Data;
using PhotoOrganizer.Infrastructure.Repositories;

namespace PhotoOrganizer.Application.Tests;

public class AlbumRepositoryTests
{
    [Fact]
    public async Task AlbumsAndPhotos_PersistManyToManyMemberships()
    {
        var options = new DbContextOptionsBuilder<PhotoOrganizerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var photo = new Photo
        {
            FileName = "shared.jpg",
            OriginalPath = "C:\\Photos\\shared.jpg"
        };
        var firstAlbum = new Album { Name = "First" };
        var secondAlbum = new Album { Name = "Second" };
        firstAlbum.AlbumPhotos.Add(new AlbumPhoto { Album = firstAlbum, Photo = photo, SortOrder = 0 });
        secondAlbum.AlbumPhotos.Add(new AlbumPhoto { Album = secondAlbum, Photo = photo, SortOrder = 1 });

        await using (var context = new PhotoOrganizerDbContext(options))
        {
            context.Albums.AddRange(firstAlbum, secondAlbum);
            await context.SaveChangesAsync();
        }

        await using var reloadedContext = new PhotoOrganizerDbContext(options);
        var repository = new AlbumRepository(reloadedContext);
        var reloadedFirst = await repository.GetAlbumAsync(firstAlbum.Id);
        var reloadedSecond = await repository.GetAlbumAsync(secondAlbum.Id);

        Assert.Equal(photo.Id, Assert.Single(reloadedFirst!.AlbumPhotos).PhotoId);
        Assert.Equal(photo.Id, Assert.Single(reloadedSecond!.AlbumPhotos).PhotoId);
    }
}

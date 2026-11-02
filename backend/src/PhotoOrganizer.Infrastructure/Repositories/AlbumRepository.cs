using Microsoft.EntityFrameworkCore;
using PhotoOrganizer.Application.Abstractions;
using PhotoOrganizer.Domain;
using PhotoOrganizer.Infrastructure.Data;

namespace PhotoOrganizer.Infrastructure.Repositories;

public class AlbumRepository(PhotoOrganizerDbContext dbContext) : IAlbumRepository
{
    public async Task<IReadOnlyList<Album>> GetAlbumsAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Albums
            .Include(album => album.AlbumPhotos)
            .ThenInclude(membership => membership.Photo)
            .OrderBy(album => album.StartDate ?? DateTime.MaxValue)
            .ThenBy(album => album.Name)
            .ToListAsync(cancellationToken);

    public Task<Album?> GetAlbumAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Albums
            .Include(album => album.AlbumPhotos)
            .ThenInclude(membership => membership.Photo)
            .FirstOrDefaultAsync(album => album.Id == id, cancellationToken);

    public Task<Photo?> GetPhotoAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Photos.FirstOrDefaultAsync(photo => photo.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Photo>> GetPhotosAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Photos.OrderBy(photo => photo.CaptureDate ?? DateTime.MaxValue)
            .ThenBy(photo => photo.FileName)
            .ToListAsync(cancellationToken);

    public async Task<Album> SaveAlbumAsync(Album album, CancellationToken cancellationToken = default)
    {
        if (dbContext.Entry(album).State == EntityState.Detached)
        {
            if (await dbContext.Albums.AnyAsync(item => item.Id == album.Id, cancellationToken))
                dbContext.Albums.Update(album);
            else
                await dbContext.Albums.AddAsync(album, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return album;
    }

    public async Task SavePhotosAsync(IReadOnlyCollection<Photo> photos, CancellationToken cancellationToken = default)
    {
        foreach (var photo in photos)
        {
            if (dbContext.Entry(photo).State != EntityState.Detached)
                continue;

            if (await dbContext.Photos.AnyAsync(existing => existing.Id == photo.Id, cancellationToken))
                dbContext.Photos.Update(photo);
            else
                await dbContext.Photos.AddAsync(photo, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SynchronizeDateGroupsAsync(
        IReadOnlyCollection<Album> groups,
        CancellationToken cancellationToken = default)
    {
        var currentGroups = await dbContext.Albums
            .Where(album => album.IsDateGroup)
            .Include(album => album.AlbumPhotos)
            .ToListAsync(cancellationToken);

        var activeGroupIds = new HashSet<Guid>();
        foreach (var group in groups)
        {
            var current = currentGroups.FirstOrDefault(album =>
                album.StartDate == group.StartDate && album.EndDate == group.EndDate);
            if (current is null)
            {
                current = new Album { Name = group.Name, IsDateGroup = true };
                await dbContext.Albums.AddAsync(current, cancellationToken);
                currentGroups.Add(current);
            }
            activeGroupIds.Add(current.Id);

            current.StartDate = group.StartDate;
            current.EndDate = group.EndDate;
            current.UpdatedAt = DateTime.UtcNow;
            current.CoverPhotoId = group.AlbumPhotos.FirstOrDefault()?.PhotoId;
            current.CoverImagePath = group.AlbumPhotos.FirstOrDefault()?.Photo.OriginalPath;
            var desiredPhotoIds = group.AlbumPhotos.Select(membership => membership.PhotoId).ToHashSet();
            foreach (var existing in current.AlbumPhotos.Where(membership => !desiredPhotoIds.Contains(membership.PhotoId)).ToArray())
                current.AlbumPhotos.Remove(existing);

            foreach (var membership in group.AlbumPhotos)
            {
                var existing = current.AlbumPhotos.FirstOrDefault(item => item.PhotoId == membership.PhotoId);
                if (existing is not null)
                {
                    existing.SortOrder = membership.SortOrder;
                    existing.AssignDateUtc = membership.AssignDateUtc;
                }
                else
                {
                    current.AlbumPhotos.Add(new AlbumPhoto
                    {
                        AlbumId = current.Id,
                        PhotoId = membership.PhotoId,
                        Photo = membership.Photo,
                        SortOrder = membership.SortOrder,
                        AssignDateUtc = membership.AssignDateUtc
                    });
                }
            }
        }

        dbContext.Albums.RemoveRange(currentGroups.Where(album => !activeGroupIds.Contains(album.Id)));
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

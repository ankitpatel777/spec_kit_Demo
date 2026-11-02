using PhotoOrganizer.Domain;

namespace PhotoOrganizer.Application.Abstractions;

public interface IAlbumRepository
{
    Task<IReadOnlyList<Album>> GetAlbumsAsync(CancellationToken cancellationToken = default);
    Task<Album?> GetAlbumAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Photo?> GetPhotoAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Photo>> GetPhotosAsync(CancellationToken cancellationToken = default);
    Task<Album> SaveAlbumAsync(Album album, CancellationToken cancellationToken = default);
    Task SavePhotosAsync(IReadOnlyCollection<Photo> photos, CancellationToken cancellationToken = default);
    Task SynchronizeDateGroupsAsync(IReadOnlyCollection<Album> groups, CancellationToken cancellationToken = default);
}

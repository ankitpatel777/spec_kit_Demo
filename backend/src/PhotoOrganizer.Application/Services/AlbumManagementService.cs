using PhotoOrganizer.Application.Abstractions;
using PhotoOrganizer.Application.DTOs;
using PhotoOrganizer.Domain;

namespace PhotoOrganizer.Application.Services;

public class AlbumManagementService(IAlbumRepository repository)
{
    public async Task<Album> CreateAlbumAsync(
        CreateAlbumRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateName(request.Name);
        ValidateDateRange(request.StartDateUtc?.UtcDateTime, request.EndDateUtc?.UtcDateTime);
        var album = new Album
        {
            Name = request.Name.Trim(),
            StartDate = request.StartDateUtc?.UtcDateTime,
            EndDate = request.EndDateUtc?.UtcDateTime,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        return await repository.SaveAlbumAsync(album, cancellationToken);
    }

    public async Task<Album?> UpdateAlbumAsync(
        Guid albumId,
        UpdateAlbumRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Name is not null)
            ValidateName(request.Name);

        var album = await repository.GetAlbumAsync(albumId, cancellationToken);
        if (album is null)
            return null;

        ValidateDateRange(
            request.StartDateUtc?.UtcDateTime ?? album.StartDate,
            request.EndDateUtc?.UtcDateTime ?? album.EndDate);
        if (request.Name is not null)
            album.Name = request.Name.Trim();
        if (request.StartDateUtc.HasValue)
            album.StartDate = request.StartDateUtc.Value.UtcDateTime;
        if (request.EndDateUtc.HasValue)
            album.EndDate = request.EndDateUtc.Value.UtcDateTime;
        if (request.CoverPhotoId.HasValue)
        {
            var cover = await repository.GetPhotoAsync(request.CoverPhotoId.Value, cancellationToken);
            if (cover is null)
                throw new ArgumentException("Cover photo does not exist.", nameof(request));
            if (!album.AlbumPhotos.Any(membership => membership.PhotoId == cover.Id))
                throw new ArgumentException("Cover photo must belong to this album.", nameof(request));
            album.CoverPhotoId = cover.Id;
        }

        album.UpdatedAt = DateTime.UtcNow;
        return await repository.SaveAlbumAsync(album, cancellationToken);
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Album name is required.", nameof(name));
        if (name.Trim().Length > 255)
            throw new ArgumentException("Album name cannot exceed 255 characters.", nameof(name));
    }

    private static void ValidateDateRange(DateTime? start, DateTime? end)
    {
        if (start.HasValue && end.HasValue && end.Value < start.Value)
            throw new ArgumentException("Album end date must be on or after its start date.");
    }
}

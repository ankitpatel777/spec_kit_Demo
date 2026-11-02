using PhotoOrganizer.Domain;

namespace PhotoOrganizer.Application.Services;

public class AlbumGroupingService
{
    public IReadOnlyList<Album> GroupPhotosByDate(IEnumerable<Photo> photos)
    {
        return photos
            .GroupBy(photo => photo.CaptureDate?.Date)
            .OrderBy(group => group.Key ?? DateTime.MaxValue)
            .Select(group =>
            {
                var album = new Album
                {
                    Name = group.Key?.ToString("yyyy-MM-dd") ?? "Unspecified Dates",
                    StartDate = group.Key,
                    EndDate = group.Key,
                    IsDateGroup = true
                };
                var memberships = group
                    .OrderBy(photo => photo.FileName, StringComparer.OrdinalIgnoreCase)
                    .Select((photo, index) => new AlbumPhoto
                    {
                        AlbumId = album.Id,
                        Album = album,
                        PhotoId = photo.Id,
                        Photo = photo,
                        SortOrder = index,
                        AssignDateUtc = group.Key
                    })
                    .ToArray();
                album.AlbumPhotos = memberships;
                album.CoverPhotoId = memberships.FirstOrDefault()?.PhotoId;
                album.CoverImagePath = memberships.FirstOrDefault()?.Photo.OriginalPath;
                return album;
            })
            .ToArray();
    }
}

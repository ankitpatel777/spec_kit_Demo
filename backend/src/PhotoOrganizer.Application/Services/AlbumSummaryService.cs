using PhotoOrganizer.Domain;

namespace PhotoOrganizer.Application.Services;

public class AlbumSummaryService
{
    public AlbumSummaryDto CreateSummary(Album album)
    {
        var dates = album.AlbumPhotos
            .Select(membership => membership.Photo.CaptureDate)
            .Where(date => date.HasValue)
            .Select(date => date!.Value)
            .ToArray();
        var startDate = dates.Length > 0 ? dates.Min() : album.StartDate;
        var endDate = dates.Length > 0 ? dates.Max() : album.EndDate;
        var coverPhotoId = album.CoverPhotoId ?? album.AlbumPhotos
            .OrderBy(membership => membership.SortOrder)
            .Select(membership => (Guid?)membership.PhotoId)
            .FirstOrDefault();

        return new AlbumSummaryDto
        {
            Id = album.Id,
            Name = album.Name,
            CoverPhotoId = coverPhotoId,
            CoverImageUrl = coverPhotoId.HasValue ? $"/api/photos/{coverPhotoId}/content" : null,
            StartDateUtc = startDate,
            EndDateUtc = endDate,
            DateRange = FormatDateRange(startDate, endDate),
            PhotoCount = album.AlbumPhotos.Count,
            IsEmpty = album.AlbumPhotos.Count == 0,
            IsDateGroup = album.IsDateGroup
        };
    }

    private static string FormatDateRange(DateTime? start, DateTime? end)
    {
        if (start is null || end is null)
            return "Unspecified dates";
        if (start.Value.Date == end.Value.Date)
            return start.Value.ToString("yyyy-MM-dd");
        return $"{start.Value:yyyy-MM-dd} to {end.Value:yyyy-MM-dd}";
    }
}

public class AlbumSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? CoverPhotoId { get; set; }
    public string? CoverImageUrl { get; set; }
    public DateTime? StartDateUtc { get; set; }
    public DateTime? EndDateUtc { get; set; }
    public string DateRange { get; set; } = string.Empty;
    public int PhotoCount { get; set; }
    public bool IsEmpty { get; set; }
    public bool IsDateGroup { get; set; }
}

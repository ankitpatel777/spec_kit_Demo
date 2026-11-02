namespace PhotoOrganizer.Application.DTOs;

public class UpdateAlbumRequest
{
    public string? Name { get; set; }
    public Guid? CoverPhotoId { get; set; }
    public DateTimeOffset? StartDateUtc { get; set; }
    public DateTimeOffset? EndDateUtc { get; set; }
}

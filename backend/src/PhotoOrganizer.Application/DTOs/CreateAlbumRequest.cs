namespace PhotoOrganizer.Application.DTOs;

public class CreateAlbumRequest
{
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset? StartDateUtc { get; set; }
    public DateTimeOffset? EndDateUtc { get; set; }
}

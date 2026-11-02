namespace PhotoOrganizer.Application.DTOs;

public class AlbumDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? CoverPhotoId { get; set; }
    public string? CoverImageUrl { get; set; }
    public DateTime? StartDateUtc { get; set; }
    public DateTime? EndDateUtc { get; set; }
    public int PhotoCount { get; set; }
    public bool IsEmpty { get; set; }
    public bool IsDateGroup { get; set; }
    public List<PhotoDto> Photos { get; set; } = new();
}

public class PhotoDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public DateTime? TakenOnUtc { get; set; }
    public bool IsDateResolved { get; set; }
    public string ContentUrl { get; set; } = string.Empty;
}

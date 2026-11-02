namespace PhotoOrganizer.Domain;

public class Photo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; } = string.Empty;
    public string OriginalPath { get; set; } = string.Empty;
    public DateTime? CaptureDate { get; set; }
    public DateTime ImportedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime FileModifiedAtUtc { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public bool IsDateResolved => CaptureDate.HasValue;
    public string? Hash { get; set; }
    public string? ContentType { get; set; }
    public long FileSizeBytes { get; set; }
    public ICollection<AlbumPhoto> AlbumPhotos { get; set; } = new List<AlbumPhoto>();
}

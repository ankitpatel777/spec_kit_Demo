namespace PhotoOrganizer.Domain;

public class Album
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public Guid? CoverPhotoId { get; set; }
    public string? CoverImagePath { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDateGroup { get; set; }
    public ICollection<AlbumPhoto> AlbumPhotos { get; set; } = new List<AlbumPhoto>();

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public IReadOnlyCollection<Photo> Photos => AlbumPhotos.Select(membership => membership.Photo).ToArray();
}

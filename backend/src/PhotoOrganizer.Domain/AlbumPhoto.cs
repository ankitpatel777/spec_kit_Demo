namespace PhotoOrganizer.Domain;

public class AlbumPhoto
{
    public Guid AlbumId { get; set; }
    public Album Album { get; set; } = null!;
    public Guid PhotoId { get; set; }
    public Photo Photo { get; set; } = null!;
    public int SortOrder { get; set; }
    public DateTime? AssignDateUtc { get; set; }
}

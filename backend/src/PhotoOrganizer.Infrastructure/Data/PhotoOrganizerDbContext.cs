using Microsoft.EntityFrameworkCore;
using PhotoOrganizer.Domain;

namespace PhotoOrganizer.Infrastructure.Data;

public class PhotoOrganizerDbContext(DbContextOptions<PhotoOrganizerDbContext> options) : DbContext(options)
{
    public DbSet<Photo> Photos => Set<Photo>();
    public DbSet<Album> Albums => Set<Album>();
    public DbSet<AlbumPhoto> AlbumPhotos => Set<AlbumPhoto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Album>(entity =>
        {
            entity.Property(album => album.Name).HasMaxLength(255).IsRequired();
            entity.Property(album => album.CoverImagePath).HasMaxLength(1024);
            entity.HasOne<Photo>()
                .WithMany()
                .HasForeignKey(album => album.CoverPhotoId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasMany(album => album.AlbumPhotos)
                .WithOne(membership => membership.Album)
                .HasForeignKey(membership => membership.AlbumId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Photo>(entity =>
        {
            entity.Property(photo => photo.FileName).HasMaxLength(255).IsRequired();
            entity.Property(photo => photo.OriginalPath).HasMaxLength(1024).IsRequired();
            entity.Property(photo => photo.Hash).HasMaxLength(128);
            entity.Property(photo => photo.ContentType).HasMaxLength(128);
            entity.HasIndex(photo => photo.OriginalPath).IsUnique();
            entity.HasMany(photo => photo.AlbumPhotos)
                .WithOne(membership => membership.Photo)
                .HasForeignKey(membership => membership.PhotoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AlbumPhoto>(entity =>
        {
            entity.HasKey(membership => new { membership.AlbumId, membership.PhotoId });
            entity.HasIndex(membership => membership.PhotoId);
            entity.ToTable(table => table.HasCheckConstraint(
                "CK_AlbumPhotos_SortOrder",
                "[SortOrder] >= 0"));
        });
    }
}

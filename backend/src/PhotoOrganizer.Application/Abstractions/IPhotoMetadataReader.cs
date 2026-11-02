namespace PhotoOrganizer.Application.Abstractions;

public interface IPhotoMetadataReader
{
    DateTime? ReadCaptureDateUtc(string filePath);
}

namespace PhotoOrganizer.Application.Abstractions;

public interface ILocalFileStorage
{
    IReadOnlyList<string> EnumerateImages();
    bool IsInsideLibrary(string path);
}

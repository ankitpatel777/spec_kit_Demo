using Microsoft.AspNetCore.Mvc;
using PhotoOrganizer.Application.Abstractions;
using PhotoOrganizer.Application.DTOs;
using PhotoOrganizer.Application.Services;

namespace PhotoOrganizer.Api.Controllers;

[ApiController]
[Route("api/photos")]
public class PhotoController(
    IAlbumRepository repository,
    ILocalFileStorage fileStorage,
    PhotoLibraryService photoLibraryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PhotoDto>>> GetPhotos(CancellationToken cancellationToken)
    {
        var photos = await repository.GetPhotosAsync(cancellationToken);
        return Ok(photos.Select(photo => new PhotoDto
        {
            Id = photo.Id,
            FileName = photo.FileName,
            TakenOnUtc = photo.CaptureDate,
            IsDateResolved = photo.IsDateResolved,
            ContentUrl = $"/api/photos/{photo.Id}/content"
        }));
    }

    [HttpPost("import")]
    public async Task<ActionResult<object>> ImportPhotos(CancellationToken cancellationToken)
    {
        var importedCount = await photoLibraryService.ImportLocalPhotosAsync(cancellationToken);
        return Ok(new { importedCount });
    }

    [HttpGet("{photoId:guid}/content")]
    public async Task<IActionResult> GetContent(Guid photoId, CancellationToken cancellationToken)
    {
        var photo = await repository.GetPhotoAsync(photoId, cancellationToken);
        if (photo is null || !fileStorage.IsInsideLibrary(photo.OriginalPath) || !System.IO.File.Exists(photo.OriginalPath))
            return NotFound();

        return PhysicalFile(photo.OriginalPath, photo.ContentType ?? "application/octet-stream");
    }
}

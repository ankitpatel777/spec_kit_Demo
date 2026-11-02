using Microsoft.AspNetCore.Mvc;
using PhotoOrganizer.Application.Abstractions;
using PhotoOrganizer.Application.DTOs;
using PhotoOrganizer.Application.Services;
using PhotoOrganizer.Domain;

namespace PhotoOrganizer.Api.Controllers;

[ApiController]
[Route("api/albums")]
public class AlbumController(
    IAlbumRepository repository,
    AlbumManagementService managementService,
    AlbumSummaryService summaryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AlbumSummaryDto>>> GetAlbums(CancellationToken cancellationToken)
    {
        var albums = await repository.GetAlbumsAsync(cancellationToken);
        return Ok(albums.Select(summaryService.CreateSummary));
    }

    [HttpGet("{albumId:guid}")]
    public async Task<ActionResult<AlbumDto>> GetAlbum(Guid albumId, CancellationToken cancellationToken)
    {
        var album = await repository.GetAlbumAsync(albumId, cancellationToken);
        return album is null ? NotFound() : Ok(ToDto(album));
    }

    [HttpGet("{albumId:guid}/photos")]
    public async Task<ActionResult<IEnumerable<PhotoDto>>> GetAlbumPhotos(
        Guid albumId,
        CancellationToken cancellationToken)
    {
        var album = await repository.GetAlbumAsync(albumId, cancellationToken);
        return album is null
            ? NotFound()
            : Ok(album.AlbumPhotos.OrderBy(membership => membership.SortOrder)
                .Select(membership => ToPhotoDto(membership.Photo)));
    }

    [HttpPost]
    public async Task<ActionResult<AlbumDto>> CreateAlbum(
        CreateAlbumRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var album = await managementService.CreateAlbumAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetAlbum), new { albumId = album.Id }, ToDto(album));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Title = "Invalid album", Detail = exception.Message });
        }
    }

    [HttpPatch("{albumId:guid}")]
    public async Task<ActionResult<AlbumDto>> UpdateAlbum(
        Guid albumId,
        UpdateAlbumRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var album = await managementService.UpdateAlbumAsync(albumId, request, cancellationToken);
            return album is null ? NotFound() : Ok(ToDto(album));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Title = "Invalid album", Detail = exception.Message });
        }
    }

    private static AlbumDto ToDto(Album album) => new()
    {
        Id = album.Id,
        Name = album.Name,
        CoverPhotoId = album.CoverPhotoId,
        CoverImageUrl = album.CoverPhotoId.HasValue
            ? $"/api/photos/{album.CoverPhotoId}/content"
            : null,
        StartDateUtc = album.StartDate,
        EndDateUtc = album.EndDate,
        PhotoCount = album.AlbumPhotos.Count,
        IsEmpty = album.AlbumPhotos.Count == 0,
        IsDateGroup = album.IsDateGroup,
        Photos = album.AlbumPhotos.OrderBy(membership => membership.SortOrder)
            .Select(membership => ToPhotoDto(membership.Photo))
            .ToList()
    };

    private static PhotoDto ToPhotoDto(Photo photo) => new()
    {
        Id = photo.Id,
        FileName = photo.FileName,
        TakenOnUtc = photo.CaptureDate,
        IsDateResolved = photo.IsDateResolved,
        ContentUrl = $"/api/photos/{photo.Id}/content"
    };
}

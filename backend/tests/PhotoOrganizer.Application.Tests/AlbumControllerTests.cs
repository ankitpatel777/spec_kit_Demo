using Microsoft.AspNetCore.Mvc;
using PhotoOrganizer.Api.Controllers;
using PhotoOrganizer.Application.Abstractions;
using PhotoOrganizer.Application.DTOs;
using PhotoOrganizer.Application.Services;
using PhotoOrganizer.Domain;

namespace PhotoOrganizer.Application.Tests;

public class AlbumControllerTests
{
    [Fact]
    public async Task GetAlbums_ReturnsPersistedAlbumSummaries()
    {
        var repository = new TestAlbumRepository();
        await repository.SaveAlbumAsync(new Album { Name = "Weekend", StartDate = new DateTime(2026, 6, 1) });
        var controller = CreateController(repository);

        var action = await controller.GetAlbums(CancellationToken.None);

        var albums = Assert.IsType<OkObjectResult>(action.Result).Value as IEnumerable<AlbumSummaryDto>;
        Assert.Equal("Weekend", Assert.Single(albums!).Name);
    }

    [Fact]
    public async Task CreateAlbum_RejectsBlankName()
    {
        var controller = CreateController(new TestAlbumRepository());

        var action = await controller.CreateAlbum(new CreateAlbumRequest { Name = " " }, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(action.Result);
    }

    [Fact]
    public async Task GetAlbum_ReturnsNotFoundForUnknownAlbum()
    {
        var controller = CreateController(new TestAlbumRepository());

        var action = await controller.GetAlbum(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(action.Result);
    }

    private static AlbumController CreateController(TestAlbumRepository repository)
        => new(repository, new AlbumManagementService(repository), new AlbumSummaryService());
}

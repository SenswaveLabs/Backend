using Microsoft.Extensions.Logging;
using Moq;
using Senswave.Homes.Application.Homes.Features.GetCurrentHome;
using Senswave.Homes.Domain.Homes.Entities;
using Senswave.Homes.Domain.Homes.Features.GetCurrentHome;
using Senswave.Homes.Domain.Homes.ValueObjects;

namespace Senswave.Homes.UnitTests.Homes.GetCurrentHome;

[Trait("Collection", "UnitTests")]
public class GetCurrentHomeQueryHandlerTests
{
    private readonly Mock<IGetCurrentHomeRepository> _homeRepositoryMock = new();
    private readonly Mock<ILogger<GetCurrentHomeHandler>> _loggerMock = new();

    private readonly GetCurrentHomeHandler _handler;

    public GetCurrentHomeQueryHandlerTests()
    {
        _handler = new GetCurrentHomeHandler(
            _homeRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CurrentHomeReturnsHomeOwnedByUserWhenNoCoordinatesProvided()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var homes = new List<Home>
        {
            new() { Id = Guid.Parse("a2d8d472-3004-455a-acb5-3905e8e30d26"), OwnerId = userId, CreatedAtUtc = DateTime.UtcNow.AddDays(-1) },
            new() { Id = Guid.Parse("b2d8d472-3004-455a-acb5-3905e8e30d26"), OwnerId = userId, CreatedAtUtc = DateTime.UtcNow },

            new() { Id = Guid.Parse("c2d8d472-3004-455a-acb5-3905e8e30d26"), OwnerId = Guid.NewGuid(), CreatedAtUtc = DateTime.UtcNow.AddDays(-1) },
            new() { Id = Guid.Parse("d2d8d472-3004-455a-acb5-3905e8e30d26"), OwnerId = Guid.NewGuid(), CreatedAtUtc = DateTime.UtcNow }
        };

        _homeRepositoryMock.Setup(repo => repo.GetHomes(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(homes);

        var query = new GetCurrentHomeQuery { UserId = userId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(userId, result.Data.OwnerId);
    }


    [Fact]
    public async Task CurrentHomeReturnsHomeThatIamInWhenCoordinatesProvidedAndDistanceValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var homes = new List<Home>
        {
            new() { Id = Guid.Parse("b2d8d472-3004-455a-acb5-3905e8e30d26"), Location = new Location { Latitude = 0, Longitude = 0 }, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.Parse("a2d8d472-3004-455a-acb5-3905e8e30d26"), Location = new Location { Latitude = 3, Longitude = 1 }, CreatedAtUtc = DateTime.UtcNow.AddDays(-1) },
            new() { Id = Guid.Parse("c2d8d472-3004-455a-acb5-3905e8e30d26"), Location = new Location { Latitude = 3, Longitude = -6 }, CreatedAtUtc = DateTime.UtcNow.AddDays(-2) }
        };

        _homeRepositoryMock.Setup(repo => repo.GetHomes(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(homes);

        var query = new GetCurrentHomeQuery { UserId = userId, Longitude = 0, Latitude = 0 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(homes[0].Id, result.Data.Id);
    }

    [Fact]
    public async Task CurrentHomeReturnsNullWhenNoHomesAvailable()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _homeRepositoryMock.Setup(repo => repo.GetHomes(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var query = new GetCurrentHomeQuery { UserId = userId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task CurrentHomeReturnsOldestWhenNoLocalizationForHomes()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var homes = new List<Home>
        {
            new() { Id = Guid.Parse("a2d8d472-3004-455a-acb5-3905e8e30d26"), OwnerId = userId, CreatedAtUtc = DateTime.UtcNow.AddDays(-1) },
            new() { Id = Guid.Parse("b2d8d472-3004-455a-acb5-3905e8e30d26"), OwnerId = userId, CreatedAtUtc = DateTime.UtcNow},
            new() { Id = Guid.Parse("c2d8d472-3004-455a-acb5-3905e8e30d26"), OwnerId = userId, CreatedAtUtc = DateTime.UtcNow.AddDays(-5) }
        };

        _homeRepositoryMock.Setup(repo => repo.GetHomes(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(homes);

        var query = new GetCurrentHomeQuery { UserId = userId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(homes[2], result.Data);
    }

    [Fact]
    public async Task ReturnsClosestCurrentHome()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var homes = new List<Home>
        {
            new() { Id = Guid.Parse("b2d8d472-3004-455a-acb5-3905e8e30d26"), Location = new Location { Latitude = 45, Longitude = 45 }, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.Parse("a2d8d472-3004-455a-acb5-3905e8e30d26"), Location = new Location { Latitude = 3, Longitude = 1 }, CreatedAtUtc = DateTime.UtcNow.AddDays(-1) },
            new() { Id = Guid.Parse("c2d8d472-3004-455a-acb5-3905e8e30d26"), Location = new Location { Latitude = 3, Longitude = -6 }, CreatedAtUtc = DateTime.UtcNow.AddDays(-2) }
        };

        _homeRepositoryMock.Setup(repo => repo.GetHomes(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(homes);

        var query = new GetCurrentHomeQuery
        {
            UserId = userId,
            Latitude = 44.9995,
            Longitude = 45.001
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(homes[0], result.Data);
    }
}
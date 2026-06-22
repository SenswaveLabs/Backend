using Microsoft.Extensions.Logging;
using Moq;
using Senswave.Abstractions.Resulting;
using Senswave.Homes.Application.Homes.Features.UpdateHome;
using Senswave.Homes.Domain.Homes.Entities;
using Senswave.Homes.Domain.Homes.Repositories;
using Senswave.Homes.Domain.Homes.Services;
using Senswave.Homes.Domain.Homes.ValueObjects;
using Senswave.Homes.Domain.Sharings.Entities;


namespace Senswave.Homes.UnitTests.Homes.PatchHome;

public class PatchHomeCommandHandlerTests
{
    private readonly Mock<IHomeAccessService> _homeAccesService = new();
    private readonly Mock<IHomeCommandRepository> _commandRepository = new();
    private readonly Mock<ILogger<UpdateHomeHandler>> _loggerMock = new();
    private readonly UpdateHomeHandler _handler;

    public PatchHomeCommandHandlerTests()
    {
        _commandRepository.Setup(x => x.UpdateHome(It.IsAny<Home>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        _handler = new UpdateHomeHandler(_homeAccesService.Object, _commandRepository.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task HomeNotFoundReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var homeId = Guid.NewGuid();
        var command = new UpdateHomeCommand { UserId = userId, HomeId = homeId };
        Home? home = null;
        _homeAccesService.Setup(x => x.CanManage(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        _commandRepository.Setup(r => r.GetHome(command.HomeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(home);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(UpdateHomeError.HomeNotFound, result.Errors);
    }

    [Fact]
    public async Task SharedHomeNameIsUpdated()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var homeId = Guid.NewGuid();
        var command = new UpdateHomeCommand { UserId = userId, HomeId = homeId, Name="Name123" };
        var sharedHome = new Home { Id = homeId };
        var homeSharing = new HomeSharing { Home = sharedHome };

        _homeAccesService.Setup(x => x.CanManage(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        _commandRepository.Setup(r => r.GetHome(command.HomeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sharedHome);

        _commandRepository.Setup(r => r.HomeExists(command.UserId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(command.Name, result.Data.Name);
    }

    [Fact]
    public async Task SharedHomeNameIsUsed()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var homeId = Guid.NewGuid();
        var command = new UpdateHomeCommand { UserId = userId, HomeId = homeId, Name="Name123" };
        var sharedHome = new Home { Id = homeId };
        var homeSharing = new HomeSharing { Home = sharedHome };
        Home? home = null;

        _homeAccesService.Setup(x => x.CanManage(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        _commandRepository.Setup(r => r.GetHome(command.HomeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(home);

        _commandRepository.Setup(r => r.HomeExists(command.UserId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task HomeNameIsUsed()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var homeId = Guid.NewGuid();
        var command = new UpdateHomeCommand { UserId = userId, HomeId = homeId, Name = "newName" };
        var home = new Home { Id = homeId, Name = "currentName" };
        _homeAccesService.Setup(x => x.CanManage(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        _commandRepository.Setup(r => r.GetHome(command.HomeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(home);
        _commandRepository.Setup(r => r.HomeExists(command.UserId, command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task HomeNameIsUpdated()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var homeId = Guid.NewGuid();
        var command = new UpdateHomeCommand { UserId = userId, HomeId = homeId, Name = "newName" };
        var home = new Home { Id = homeId, Name = "currentName" };

        _homeAccesService.Setup(x => x.CanManage(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        _commandRepository.Setup(r => r.GetHome(command.HomeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(home);
        _commandRepository.Setup(r => r.HomeExists(command.UserId, command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(command.Name, home.Name);
    }

    [Fact]
    public async Task UpdatesIcon()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var homeId = Guid.NewGuid();
        var command = new UpdateHomeCommand { UserId = userId, HomeId = homeId, Icon = "newIcon" };
        var home = new Home { Id = homeId };
        _commandRepository.Setup(r => r.GetHome(command.HomeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(home);

        _homeAccesService.Setup(x => x.CanManage(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(command.Icon, home.Icon);
    }

    [Fact]
    public async Task RemoveLocationSetsLocationToNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var homeId = Guid.NewGuid();
        var command = new UpdateHomeCommand { UserId = userId, HomeId = homeId, RemoveLocalization = true };
        var home = new Home { Id = homeId, Location = new Location { Latitude = 10, Longitude = 20 } };
        _commandRepository.Setup(r => r.GetHome(command.HomeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(home);

        _homeAccesService.Setup(x => x.CanManage(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(home.Location);
    }

    [Fact]
    public async Task UpdatesLocationIfChanged()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var homeId = Guid.NewGuid();
        var command = new UpdateHomeCommand { UserId = userId, HomeId = homeId, Latitude = 12.34, Longitude = 56.78 };
        var home = new Home { Id = homeId, Location = new Location { Latitude = 10, Longitude = 20 } };
        _commandRepository.Setup(r => r.GetHome(command.HomeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(home);

        _homeAccesService.Setup(x => x.CanManage(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(command.Latitude, home.Location.Latitude);
        Assert.Equal(command.Longitude, home.Location.Longitude);
    }

    [Fact]
    public async Task UpdatesLocationIfOneChanged()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var homeId = Guid.NewGuid();
        var command = new UpdateHomeCommand { UserId = userId, HomeId = homeId, Longitude = 56.78 };
        var home = new Home { Id = homeId, Location = new Location { Latitude = 10, Longitude = 20 } };
        _commandRepository.Setup(r => r.GetHome(command.HomeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(home);

        _homeAccesService.Setup(x => x.CanManage(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(command.Longitude, home.Location.Longitude);
    }
}

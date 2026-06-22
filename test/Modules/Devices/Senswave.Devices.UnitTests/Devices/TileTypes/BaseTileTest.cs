using Senswave.Abstractions.Messaging.Interfaces;
using Senswave.Devices.Application.Devices.Services;
using Senswave.Devices.Domain.Devices.Factory;
using Senswave.Devices.Domain.Devices.Repositories;
using Senswave.Devices.Domain.Devices.TileTypes;
using Senswave.Devices.Domain.Devices.Types;
using Senswave.Devices.Domain.Operations.Factory;
using Senswave.Devices.Domain.Operations.Types;

namespace Senswave.Devices.UnitTests.Devices.TileTypes;

public abstract class BaseTileTest
{
    protected readonly OperationFactory operationFactory;
    protected readonly DeviceTileFactory deviceTileFactory;
    protected readonly DeviceFactory deviceFactory;
    protected readonly DeviceService deviceService;
    protected readonly Mock<IDeviceQueryRepository> deviceQueryRepository;

    protected BaseTileTest()
    {
        deviceQueryRepository = new Mock<IDeviceQueryRepository>();

        operationFactory = new OperationFactory(
            new Mock<ILogger<IOperation>>().Object,
            new Mock<ILogger<OperationFactory>>().Object);

        deviceTileFactory = new DeviceTileFactory(
            operationFactory,
            new Mock<ILogger<DeviceTileFactory>>().Object,
            new Mock<ILogger<IDeviceTile>>().Object);

        deviceFactory = new DeviceFactory(
            deviceTileFactory,
            operationFactory,
            deviceQueryRepository.Object,
            new Mock<ILogger<DeviceFactory>>().Object,
            new Mock<ILogger<IDevice>>().Object);

        deviceService = new DeviceService(
            operationFactory,
            deviceFactory,
            new Mock<IPublishMessageBus>().Object,
            deviceQueryRepository.Object,
            new Mock<ILogger<DeviceService>>().Object);
    }
}

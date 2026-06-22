using Senswave.Devices.Domain.Operations.Factory;
using Senswave.Devices.Domain.Operations.Types;

namespace Senswave.Devices.UnitTests.Operations.Types;

public abstract class BaseOperationTest
{
    protected readonly OperationFactory factory;

    public BaseOperationTest()
    {
        var logger = Mock.Of<ILogger<OperationFactory>>();
        var operationLogger = Mock.Of<ILogger<IOperation>>();
        factory = new OperationFactory(operationLogger, logger);
    }
}

using Senswave.TestInfrastructure.TestEnvironments.Mqtt;

namespace Senswave.Devices.EndTests.Mqtt.Operations;

[Collection("Mqtt E2E Tests Collection")]
[Trait("Collection", "MqttEndTest")]
public class OperationValuesCleanupTests(MqttTestEnvironment factory) : MqttFeatureTest(factory)
{
    //[Fact]
    //public async Task ValuesSentByUserAreCleared()
    //{
    //    // Arrange
    //    var client = CreateClient();

    //    var putOperation = new PutOperationValueRequest
    //    {
    //        Value = "true"
    //    }.Serialize();

    //    var clients = Services.GetRequiredService<IClientService>();
    //    var options = Services.GetService<IOptions<OperationOptions>>()!.Value;

    //    var messagesToSend = options.OperationValue.MaximalNumberOfValues+2;

    //    // Act
    //    var broker = await PostMqttBroker();
    //    var home = await PostAndPutBrokerForHome(broker);
    //    var deviceId = await PostDevice(home);
    //    var operationId = await PostBooleanOperation(deviceId);

    //    await StartBrokerClient(broker);
    //    var connection = clients.GetClient(broker);

    //    await AuthorizeClientAsUser(client);

    //    var messagesSend = 0;

    //    for (int i = 0; i < messagesToSend*2; i++)
    //    {
    //        var response = await client.PutAsync($"{Paths.OperationsPath}/{operationId}/action", putOperation);

    //        if (response.IsSuccessStatusCode)
    //            messagesSend++;

    //        if (messagesSend == messagesToSend)
    //            break;

    //        await Task.Delay(500);
    //    }

    //    var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(15)).Token;
    //    var scopeFactory = Factory.Server.Services.GetRequiredService<IServiceScopeFactory>();
    //    using var scope = scopeFactory.CreateScope();

    //    var context = scope.ServiceProvider.GetRequiredService<DevicesContext>();
    //    var operation = await context.Operations
    //        .Where(x => x.Id == operationId)
    //        .Include(x => x.OperationValues)
    //        .FirstOrDefaultAsync(cancellationToken);

    //    // Assert
    //    Assert.True(messagesSend == messagesToSend, $"Failed to send messages {messagesSend}/{messagesToSend}");
    //    Assert.True(connection.IsSuccess);

    //    Assert.NotNull(operation);
    //    Assert.NotNull(operation.OperationValues);
    //    Assert.True(options.OperationValue.MaximalNumberOfValues >= operation!.OperationValues.Count, $"Too many messages in database {operation!.OperationValues.Count}");

    //    //Cleanup
    //    await StopBrokerClientInternal(broker);
    //    scope.Dispose();
    //}

    //[Fact]
    //public async Task ValuesSentByDeviceAreCleared()
    //{
    //    // Arrange
    //    var putOperation = new PutOperationValueRequest
    //    {
    //        Value = "true"
    //    }.Serialize();

    //    var clients = Services.GetRequiredService<IClientService>();
    //    var options = Services.GetService<IOptions<OperationOptions>>()!.Value;
    //    var messagesToSend = options.OperationValue.MaximalNumberOfValues + 2;

    //    var scopeFactory = Factory.Server.Services.GetRequiredService<IServiceScopeFactory>();
    //    using var scope = scopeFactory.CreateScope();

    //    var deviceDontext = scope.ServiceProvider.GetRequiredService<DevicesContext>();
    //    var context = scope.ServiceProvider.GetRequiredService<DataSourcesContext>();

    //    var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token;

    //    var operationPayload = "{\"value\":true}";

    //    // Act
    //    var broker = await PostMqttBroker();
    //    var home = await PostAndPutBrokerForHome(broker);
    //    var deviceId = await PostDevice(home);
    //    var operationId = await PostBooleanOperation(deviceId);

    //    var operation = await deviceDontext.Operations
    //        .Where(x => x.Id == operationId)
    //        .FirstOrDefaultAsync();

    //    var subscribtion = await context.Subscribtions
    //        .Where(x => x.Id == operation!.DataSourceReferenceId)
    //        .FirstOrDefaultAsync();

    //    await StartBrokerClient(broker);
    //    var connection = clients.GetClient(broker);

    //    var observer = await StartObserverClient(Guid.NewGuid().ToString(), cancellationToken);

    //    var obnserverConnected = observer.IsConnected;

    //    for (int i = 0; i < messagesToSend; i++)
    //    {
    //        var message = new MqttApplicationMessageBuilder()
    //            .WithPayload(operationPayload)
    //            .WithTopic(subscribtion!.Topic)
    //            .Build();

    //        var result = await observer.PublishAsync(message, cancellationToken);

    //        Assert.True(result.IsSuccess, $"Failed message {i}/{messagesToSend}");
    //        await Task.Delay(500);
    //    }

    //    var operationWithValues = await deviceDontext.Operations
    //        .Where(x => x.Id == operationId)
    //        .Include(x => x.OperationValues)
    //        .FirstOrDefaultAsync(cancellationToken);

    //    // Assert
    //    Assert.True(connection.IsSuccess);
    //    Assert.True(obnserverConnected);

    //    Assert.NotNull(operationWithValues);
    //    Assert.NotNull(operationWithValues!.OperationValues);
    //    Assert.True(options.OperationValue.MaximalNumberOfValues >= operationWithValues!.OperationValues.Count, $"Too many messages in database {operationWithValues!.OperationValues.Count}");

    //    //Cleanup
    //    await MqttHelpers.Cleanup(observer, cancellationToken);
    //    await StopBrokerClientInternal(broker);
    //    scope.Dispose();
    //}
}

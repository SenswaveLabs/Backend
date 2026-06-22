using Senswave.Abstractions.Messaging.Enums;
using Senswave.Infrastructure.Messaging.Extensions;

namespace Senswave.Infrastructure.Messaging.Options;

public class MessageBusOptions
{
    public const string SectionName = "MessageBus";
    public string Type { get; set; } = string.Empty;
    public MessagingProviderType ProviderType => Type.ToType();

    public KafkaOptions Kafka { get; set; } = new();
    public RabbitMqOptions RabbitMq { get; set; } = new();

}

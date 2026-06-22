using Senswave.Abstractions.Messaging.Enums;

namespace Senswave.Infrastructure.Messaging.Extensions;

internal static class MessagingProviderTypeExtenstions
{
    public static MessagingProviderType ToType(this string stringType) => stringType switch
    {
        "InMemory" => MessagingProviderType.InMemory,
        "RabbitMq" => MessagingProviderType.RabbitMq,
        "Kafka" => MessagingProviderType.Kafka,
        _ => MessagingProviderType.Invalid
    };
}

namespace Senswave.DataSources.Application.Brokers.Brokers.Features.GetBrokers;

public static class GetBrokersErrors
{
    public static Error BrokersNotFound => Error.NotFound("BrokersNotFound", "No brokers found.");
}

using System.Text.Json;

namespace Senswave.Devices.Domain.Operations.Types.Base;

internal static class BaseOperationExtensions
{
    internal static string ToJsonStringPayload(this BaseOperationConfiguration configuration, object endValue)
    {
        JsonObject? payload = null;

        var names = configuration.JsonNames;

        for (int i = names.Length - 1; i >= 0; i--)
        {
            if (payload is null)
            {
                if (endValue is bool boolValue)
                {
                    payload = new JsonObject { [names[i]] = boolValue };
                }
                else if (endValue is string stringValue)
                {
                    payload = new JsonObject { [names[i]] = stringValue };
                }
                else if (endValue is int intValue)
                {
                    payload = new JsonObject { [names[i]] = intValue };
                }
                else if (endValue is double doubleValue)
                {
                    payload = new JsonObject { [names[i]] = doubleValue };
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported type for endValue: {endValue.GetType().Name}");
                }
            }
            else
            {
                payload = new JsonObject
                {
                    [names[i]] = payload
                };
            }
        }

        return payload!.ToJsonString();
    }

    internal static JsonObject ToBaseConfiguration(this BaseOperationConfiguration configuration) => new()
    {
        ["isJson"] = configuration.IsJson,
        ["jsonNames"] = new JsonArray(configuration.JsonNames.Select(x => JsonValue.Create(x)).ToArray()),
        ["saveOnUserAction"] = configuration.SaveOnUserAction
    };

    internal static Result<JsonNode> FromJsonStringPayload(this BaseOperationConfiguration configuration, string payload)
    {
        try
        {
            var jsonPayload = JsonNode.Parse(payload)!.AsObject()!;

            var last = configuration.JsonNames[^1];

            foreach (var name in configuration.JsonNames[..^1])
            {
                jsonPayload = jsonPayload![name] as JsonObject;

                if (jsonPayload == null)
                    Result<JsonNode>.Failure(Error.Failure("FailedToReadFieldFromJson", "Failed to read proper value from json."));
            }

            var result = jsonPayload![last];

            if (result == null)
                Result<JsonNode>.Failure(Error.Failure("FailedToReadFieldFromJson", "Failed to read proper value from json."));

            return Result<JsonNode>.Success(result!);
        }
        catch (JsonException)
        {
            return Result<JsonNode>.Failure(Error.Failure("FailedToReadJson", "Received payload is not valid json."));
        }
    }
}

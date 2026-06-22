using Senswave.Abstractions.Resulting;
using System.Text.Json.Serialization;

namespace Senswave.Infrastructure.Web.Endpoints;

public class ErrorProblemDetails
{
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; } = -1;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("errors")]
    public List<Error> Errors { get; set; } = [];

    [JsonPropertyName("traceId")]
    public string TraceId { get; set; } = string.Empty;
}

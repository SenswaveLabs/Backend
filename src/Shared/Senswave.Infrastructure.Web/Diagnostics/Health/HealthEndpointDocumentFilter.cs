using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Senswave.Infrastructure.Web.Diagnostics.Health;

public class HealthEndpointDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        if (swaggerDoc.Info.Version != DiagnosticModule.GroupName)
            return;

        swaggerDoc.Paths.Add("/api/health", new OpenApiPathItem
        {
            Operations = new Dictionary<HttpMethod, OpenApiOperation>
            {
                [HttpMethod.Get] = new OpenApiOperation
                {
                    Summary = "Health Check",
                    Description = "Returns application health status in HealthChecks.UI format. Endpoint works on internal port, on external port it requires proper header to authorize.",
                    Tags = new HashSet<OpenApiTagReference>
                        {
                            new("System")
                        },
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse { Description = "Healthy" },
                        ["503"] = new OpenApiResponse { Description = "Unhealthy" }
                    }
                }
            }
        });
    }
}
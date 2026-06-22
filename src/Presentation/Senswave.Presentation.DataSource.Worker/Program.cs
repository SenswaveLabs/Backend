using Asp.Versioning;
using MassTransit;
using Microsoft.OpenApi;
using Senswave.DataSources.BrokerConnection;
using Senswave.DataSources.BrokerConnection.Features.Terminate;
using Senswave.DataSources.Domain;
using Senswave.Infrastructure;
using Senswave.Infrastructure.Diagnostics.OpenTelemetry;
using Senswave.Infrastructure.Web.Diagnostics;
using Senswave.Infrastructure.Web.Diagnostics.Health;
using Senswave.Infrastructure.Web.Exceptions;
using Senswave.Presentation.DataSource.Worker;
using Senswave.Presentation.DataSource.Worker.Diagnostics;
using Senswave.Presentation.DataSource.Worker.Public;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<BrokerWorkerLiveness>();
builder.Services.AddSimpleInfrastructure(builder.Configuration);
builder.Services.AddDataSourcesDomain(builder.Configuration);
builder.Services.AddBrokerClientServices(builder.Configuration);
builder.Services.AddDiagnostics(builder.Configuration);
builder.Services.AddPublicModule(builder.Configuration);
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(TerminateHandler).Assembly);
});

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);

    var otel = context.Configuration
        .GetSection(OpenTelemetryOptions.SectionName)
        .Get<OpenTelemetryOptions>();

    if (otel?.Enabled == true)
    {
        var url = context.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];

        if (string.IsNullOrEmpty(url))
            throw new ConfigurationException("Invalid url for OpenTelemetry");

        configuration.WriteTo.OpenTelemetry(opts =>
        {
            opts.Endpoint = url;
            opts.ResourceAttributes = new Dictionary<string, object>
            {
                ["service.name"] = context.Configuration["OTEL_SERVICE_NAME"]!
            };
        });
    }
});

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"));
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(x => x.FullName);

    options.SwaggerDoc(DiagnosticModule.GroupName, new OpenApiInfo
    {
        Title = "Diagnostics API",
        Version = DiagnosticModule.GroupName
    });

    options.SwaggerDoc(PublicModule.GroupName, new OpenApiInfo
    {
        Title = "Public API",
        Version = PublicModule.GroupName
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
             "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
             "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
             "Example: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = []
    });

    options.DocInclusionPredicate((docName, apiDesc) =>
    {
        if (!apiDesc.TryGetMethodInfo(out var methodInfo))
            return false;

        var tags = apiDesc.ActionDescriptor.EndpointMetadata
            .OfType<TagsAttribute>()
            .SelectMany(attr => attr.Tags)
            .ToList();

        if (tags.Count==0)
            return false;

        var prefixByDocument = new Dictionary<string, string>
                {
                    { PublicModule.GroupName, PublicModule.ModuleName }
                };

        if (!prefixByDocument.TryGetValue(docName, out var requiredPrefix))
            return false;

        return tags.Any(tag => tag.StartsWith(requiredPrefix, StringComparison.OrdinalIgnoreCase));
    });

    options.DocumentFilter<HealthEndpointDocumentFilter>();
});
builder.Services.AddExceptionHandler<GlobalExceptionHandlerMiddleware>();
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint($"/swagger/{DiagnosticModule.GroupName}/swagger.json", "Diagnostics REST API");
        options.SwaggerEndpoint($"/swagger/{PublicModule.GroupName}/swagger.json", "Public REST API");
    });
}
else
{
    app.UseExceptionHandler();
}

app.UseMiddleware<HealthCheckMiddleware>();
app.MapControllers();

app.Run();

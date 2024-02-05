using Microsoft.Extensions.Configuration;

namespace Ecom.AppHost;

public static class Extensions
{
    public static IResourceBuilder<ProjectResource> AddEnvironmentVariablesForOtelExporters(
        this IResourceBuilder<ProjectResource> resource,
        IConfiguration configuration)
    {
        // dynatrace
        resource
            .WithEnvironment(
                "OpenTelemetry:Dynatrace:Endpoint",
                configuration.GetValue<string>("OpenTelemetry:Dynatrace:Endpoint"))
            .WithEnvironment(
                "OpenTelemetry:Dynatrace:ApiToken",
                configuration.GetValue<string>("OpenTelemetry:Dynatrace:ApiToken"));

        // application insights
        resource.WithEnvironment(
            "APPLICATIONINSIGHTS_CONNECTION_STRING",
            configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING"));
        
        resource.WithEnvironment(
            "Honeycomb:ApiKey",
            configuration.GetValue<string>("Honeycomb:ApiKey"));

        return resource;
    }
}
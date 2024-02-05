using Microsoft.Extensions.Configuration;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace OtelExporters.Extensions;

public class DynatraceOptions
{
    public string ApiToken { get; set; }
    public string Endpoint { get; set; }
}

public static class OpenTelemetryConfigurationExtensions
{
    public const string OpenTelemetryConfigurationSection = "OpenTelemetry";
    public const string DynatraceConfigurationSection = $"{OpenTelemetryConfigurationSection}:Dynatrace";
    
    public static TracerProviderBuilder AddDynatraceExporter(this TracerProviderBuilder builder,
        IConfiguration configuration)
    {
        DynatraceOptions dynatraceOptions = new DynatraceOptions();
        configuration.Bind(DynatraceConfigurationSection, dynatraceOptions);
        return builder.AddOtlpExporter(exporterOptions =>
        {
            exporterOptions.Endpoint = new Uri($"{dynatraceOptions.Endpoint}/v1/traces");
            exporterOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
            exporterOptions.Headers = $"Authorization=Api-Token {dynatraceOptions.ApiToken}";
        });
    }
    
    public static MeterProviderBuilder AddDynatraceExporter(this MeterProviderBuilder builder,
        IConfiguration configuration)
    {
        DynatraceOptions dynatraceOptions = new DynatraceOptions();
        configuration.Bind(DynatraceConfigurationSection, dynatraceOptions);
        return builder.AddOtlpExporter((exporterOptions, readerOptions) =>
        {
            exporterOptions.Endpoint = new Uri($"{dynatraceOptions.Endpoint}/v1/metrics");
            exporterOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
            exporterOptions.Headers = $"Authorization=Api-Token {dynatraceOptions.ApiToken}";
            readerOptions.TemporalityPreference = MetricReaderTemporalityPreference.Delta;
        });
    }
    
    public static OpenTelemetryLoggerOptions AddDynatraceExporter(this OpenTelemetryLoggerOptions options,
        IConfiguration configuration)
    {
        DynatraceOptions dynatraceOptions = new DynatraceOptions();
        configuration.Bind(DynatraceConfigurationSection, dynatraceOptions);
        return options.AddOtlpExporter((exporterOptions, _) =>
        {
            exporterOptions.Endpoint = new Uri($"{dynatraceOptions.Endpoint}/v1/logs");
            exporterOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
            exporterOptions.Headers = $"Authorization=Api-Token {dynatraceOptions.ApiToken}";
        });
    }
}
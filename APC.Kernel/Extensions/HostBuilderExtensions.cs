using APC.Kernel.Registrations;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

namespace APC.Kernel.Extensions;

public static class HostBuilderExtensions {
  public static IHostBuilder AddLogging(this IHostBuilder builder,
                                        ModuleRegistration registration) {
    builder.ConfigureLogging(logs => {
      logs.AddOpenTelemetry(otel => {
        otel.IncludeScopes = true;
        ResourceBuilder resource_builder =
          ResourceBuilder
            .CreateDefault()
            .AddService(registration.name);
        otel.SetResourceBuilder(resource_builder)
            .AddOtlpExporter(exporter => {
              exporter.Endpoint =
                new Uri(
                  Configuration.GetApcVar(
                    ApcVariable.APC_OTEL_HOST));
              exporter.Protocol =
                OtlpExportProtocol.Grpc;
            });
      });
    });
    return builder;
  }
}
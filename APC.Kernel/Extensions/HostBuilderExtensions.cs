using System;
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
    if (!Configuration.HasOtelHost()) {
      return builder;
    }

    builder.ConfigureLogging(logs => {
      logs.ClearProviders();
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
#pragma warning disable CS8604 // Possible null reference argument.
                  Configuration.GetApcVar(
                    ApcVariable.APC_OTEL_HOST));
#pragma warning restore CS8604 // Possible null reference argument.
              exporter.Protocol =
                OtlpExportProtocol.Grpc;
            })
            .AddConsoleExporter();
      });
    });
    return builder;
  }
}
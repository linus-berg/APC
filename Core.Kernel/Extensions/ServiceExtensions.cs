using Core.Kernel.Registrations;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Core.Kernel.Extensions;

public static class ServiceExtensions {
  public static IServiceCollection AddTelemetry(this IServiceCollection s,
                                                ModuleRegistration
                                                  registration) {
    if (!Configuration.HasOtelHost()) {
      return s;
    }

    void ConfigureRsc(ResourceBuilder r) {
      r.AddService(
        registration.name,
        serviceVersion: "master"
      );
      r.AddTelemetrySdk();
      r.AddEnvironmentVariableDetector();
    }

    s.AddOpenTelemetry()
     .ConfigureResource(ConfigureRsc)
     .WithTracing(
       builder => {
         builder.AddSource(DiagnosticHeaders.DefaultListenerName);
         builder.AddHttpClientInstrumentation();
         builder.AddRedisInstrumentation();
         builder.AddOtlpExporter(
           cfg => {
             cfg.Endpoint =
               new Uri(Configuration.GetBackpackVariable(CoreVariables.BP_OTEL_HOST));
             cfg.Protocol = OtlpExportProtocol.Grpc;
           }
         );
       }
     )
     .WithMetrics(
       builder => {
         builder.AddHttpClientInstrumentation();
         builder.AddRuntimeInstrumentation();
         builder.AddMeter(InstrumentationOptions.MeterName);
         builder.AddOtlpExporter(
           cfg => {
             cfg.Endpoint =
               new Uri(Configuration.GetBackpackVariable(CoreVariables.BP_OTEL_HOST));
             cfg.Protocol = OtlpExportProtocol.Grpc;
           }
         );
       }
     );
    return s;
  }
}
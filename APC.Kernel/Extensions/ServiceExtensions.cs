using APC.Kernel.Registrations;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace APC.Kernel.Extensions;

public static class ServiceExtensions {
  public static IServiceCollection AddTelemetry(this IServiceCollection s,
                                                ModuleRegistration
                                                  registration) {
    void ConfigureRsc(ResourceBuilder r) {
      r.AddService(
        registration.name,
        serviceVersion: "master");
      r.AddTelemetrySdk();
      r.AddEnvironmentVariableDetector();
    }

    s.AddOpenTelemetry().ConfigureResource(ConfigureRsc).WithTracing(
      builder => {
        builder.AddSource(DiagnosticHeaders.DefaultListenerName);
        builder.AddHttpClientInstrumentation();
        builder.AddRedisInstrumentation();
        builder.AddOtlpExporter();
      }).WithMetrics(builder => {
      builder.AddHttpClientInstrumentation();
      builder.AddRuntimeInstrumentation();
      builder.AddMeter(InstrumentationOptions.MeterName);
      builder.AddOtlpExporter();
    });
    return s;
  }
}
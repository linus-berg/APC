using APC.Kernel.Registrations;
using MassTransit.Logging;
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
        serviceVersion: "master",
        serviceInstanceId: Environment.MachineName);
    }

    s.AddOpenTelemetry().ConfigureResource(ConfigureRsc).WithTracing(
      builder => {
        builder.AddHttpClientInstrumentation();
        builder.AddRedisInstrumentation();
        builder.AddSource(DiagnosticHeaders.DefaultListenerName);
        builder.AddOtlpExporter();
      }).WithMetrics(builder => {
      builder.AddHttpClientInstrumentation();
      builder.AddOtlpExporter();
    });
    return s;
  }
}
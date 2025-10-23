using Core.Infrastructure;
using Core.Infrastructure.Services;
using Core.Kernel;
using Core.Kernel.Constants;
using Core.Kernel.Extensions;
using Core.Kernel.Registrations;
using Core.Services;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using MassTransit;
using Serilog;
using Serilog.Events;
using StackExchange.Redis;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddTelemetry(
  new ModuleRegistration(ModuleType.CORE, typeof(IHost))
);

builder.Host.UseSerilog(
  (context, configuration) => {
    configuration.Enrich.FromLogContext();
    configuration.MinimumLevel.Override("Microsoft", LogEventLevel.Information);
    configuration.WriteTo.Console();
    configuration.WriteTo.File(
      Path.Combine(
        Environment.GetEnvironmentVariable("APC_LOGS"),
        "apc_api.log"
      )
    );
  }
);

// Add services to the container.
builder.Services.AddMassTransit(
  b => {
    b.UsingRabbitMq(
      (ctx, cfg) => {
        cfg.Host(
          Configuration.GetApcVar(CoreVariables.APC_RABBIT_MQ_HOST),
          "/",
          h => {
            h.Username(Configuration.GetApcVar(CoreVariables.APC_RABBIT_MQ_USER));
            h.Password(Configuration.GetApcVar(CoreVariables.APC_RABBIT_MQ_PASS));
          }
        );
        cfg.ConfigureEndpoints(ctx);
      }
    );
  }
);
builder.Services.AddSingleton<IConnectionMultiplexer>(
  ConnectionMultiplexer.Connect(
    Configuration.GetApcVar(CoreVariables.APC_REDIS_HOST)
  )
);
builder.Services.AddScoped<IApcDatabase, MongoDatabase>();
builder.Services.AddSingleton<IApcCache, ApcCache>();
builder.Services.AddScoped<IArtifactService, ArtifactService>();

/* keycloak */
builder.Host.ConfigureKeycloakConfigurationSource();

builder.Services.AddKeycloakAuthentication(
  builder.Configuration,
  o => { o.RequireHttpsMetadata = false; }
);
builder.Services.AddKeycloakAuthorization(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseCors(
  b => {
    b.AllowAnyOrigin();
    b.AllowAnyHeader();
    b.AllowAnyMethod();
  }
);
app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
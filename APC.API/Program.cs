using APC.API;
using APC.Infrastructure;
using APC.Infrastructure.Services;
using APC.Kernel;
using APC.Kernel.Constants;
using APC.Kernel.Extensions;
using APC.Kernel.Registrations;
using APC.Services;
using MassTransit;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Serilog.Events;
using StackExchange.Redis;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddTelemetry(
  new ModuleRegistration(ModuleType.APC, typeof(IHost)));

builder.Host.UseSerilog((context, configuration) => {
  configuration.Enrich.FromLogContext();
  configuration.MinimumLevel.Override("Microsoft", LogEventLevel.Information);
  configuration.WriteTo.Console();
  configuration.WriteTo.File(
    Path.Combine(
      Environment.GetEnvironmentVariable("APC_LOGS"),
      "apc_api.log"));
});

// Add services to the container.
builder.Services.AddMassTransit(b => {
  b.UsingRabbitMq((ctx, cfg) => {
    cfg.Host(
      Configuration.GetApcVar(
        ApcVariable.APC_RABBIT_MQ_HOST), "/",
      h => {
        h.Username(
          Configuration.GetApcVar(
            ApcVariable.APC_RABBIT_MQ_USER));
        h.Password(
          Configuration.GetApcVar(
            ApcVariable.APC_RABBIT_MQ_PASS));
      });
    cfg.ConfigureEndpoints(ctx);
  });
});
builder.Services.AddSingleton<IConnectionMultiplexer>(
  ConnectionMultiplexer.Connect(
    Configuration.GetApcVar(ApcVariable.APC_REDIS_HOST)));
builder.Services.AddScoped<IApcDatabase, MongoDatabase>();
builder.Services.AddSingleton<IApcCache, ApcCache>();
builder.Services.AddScoped<IArtifactService, ArtifactService>();

/* OIDC */
builder.Services.AddOidcAuthentication();
/*******/

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
  options.ForwardedHeaders =
    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => {
  options.AddDefaultPolicy(policy => {
    policy.WithOrigins(
            Configuration
              .GetApcVar(
                ApcVariable
                  .APC_API_CORS))
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials();
  });
});
WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
app.UseForwardedHeaders();
if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI();
}


app.UseCors();
app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
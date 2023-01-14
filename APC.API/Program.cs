using APC.Infrastructure;
using APC.Infrastructure.Services;
using APC.Kernel;
using APC.Services;
using Keycloak.AuthServices.Authentication;
using MassTransit;
using StackExchange.Redis;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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

/* keycloak */
builder.Host.ConfigureKeycloakConfigurationSource();

builder.Services.AddKeycloakAuthentication(builder.Configuration,
                                           o => {
                                             o.RequireHttpsMetadata = false;
                                           });

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

app.UseCors(b => {
  b.AllowAnyOrigin();
  b.AllowAnyHeader();
  b.AllowAnyMethod();
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireAuthorization();
app.Run();
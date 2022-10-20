using APC.Infrastructure;
using APC.Kernel;
using MassTransit;
using StackExchange.Redis;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMassTransit(b => {
  b.UsingRabbitMq((ctx, cfg) => {
    cfg.Host(Configuration.GetAPCVar(ApcVariable.APC_RABBIT_MQ_HOST), "/", h => {
      h.Username(Configuration.GetAPCVar(ApcVariable.APC_RABBIT_MQ_USER));
      h.Password(Configuration.GetAPCVar(ApcVariable.APC_RABBIT_MQ_PASS));
    });
    cfg.ConfigureEndpoints(ctx);
  });
});

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(Configuration.GetAPCVar(ApcVariable.APC_REDIS_HOST)));
builder.Services.AddScoped<ApcDatabase>();
builder.Services.AddSingleton<ApcCache>();

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

app.UseAuthorization();
app.MapControllers();
app.Run();
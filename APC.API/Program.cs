using APC.Infrastructure;
using APC.Kernel;
using MassTransit;
using StackExchange.Redis;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMassTransit(b => {
  b.UsingRabbitMq((ctx, cfg) => {
    cfg.Host(Configuration.GetAPCVar(Configuration.APC_VAR.APC_RABBIT_MQ_HOST), "/", h => {
      h.Username(Configuration.GetAPCVar(Configuration.APC_VAR.APC_RABBIT_MQ_USER));
      h.Password(Configuration.GetAPCVar(Configuration.APC_VAR.APC_RABBIT_MQ_PASS));
    });
    cfg.ConfigureEndpoints(ctx);
  });
});

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(Configuration.GetAPCVar(Configuration.APC_VAR.APC_REDIS_HOST)));
builder.Services.AddScoped<Database>();
builder.Services.AddSingleton<RedisCache>();

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

if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
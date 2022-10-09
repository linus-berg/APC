using APC.Infrastructure;
using MassTransit;
using StackExchange.Redis;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMassTransit(b => {
  b.UsingRabbitMq((ctx, cfg) => {
    cfg.Host("localhost", "/", h => {
      h.Username("guest");
      h.Password("guest");
    });
    cfg.ConfigureEndpoints(ctx);
  });
});

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost"));
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
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
using Backend.Api.Infrastructure.Database;
using Backend.Api.Infrastructure.RabbitMq;
using Backend.Api.Infrastructure.Redis;
using Backend.Api.Infrastructure.Storage;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMediatR(opts => opts.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddScoped<IDatabaseHealthService, PostgresDatabaseHealthService>();
builder.Services.AddScoped<IRedisHealthService, RedisHealthService>();
builder.Services.AddScoped<IRabbitMqHealthService, RabbitMqHealthService>();
builder.Services.AddHttpClient<IMinioHealthService, MinioHealthService>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

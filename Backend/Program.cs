using Backend.Api.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Backend.Api.Infrastructure.RabbitMq;
using Backend.Api.Infrastructure.Redis;
using Backend.Api.Infrastructure.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMediatR(opts => opts.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddDbContext<FileMetadataDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));
builder.Services.AddScoped<IDatabaseHealthService, PostgresDatabaseHealthService>();
builder.Services.AddScoped<IRedisHealthService, RedisHealthService>();
builder.Services.AddScoped<IRabbitMqHealthService, RabbitMqHealthService>();
builder.Services.AddScoped<IFileLocationPublisher, RabbitMqFileLocationPublisher>();
builder.Services.AddScoped<IFileMetadataService, PostgresFileMetadataService>();
builder.Services.AddStackExchangeRedisCache(options =>
    options.Configuration = builder.Configuration.GetConnectionString("Redis"));
builder.Services.AddHttpClient<IMinioHealthService, MinioHealthService>();
builder.Services.AddScoped<IFileStorageService, MinioFileStorageService>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FileMetadataDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    await dbContext.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
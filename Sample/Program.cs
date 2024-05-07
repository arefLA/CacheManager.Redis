using System.Text.Json;
using System.Text.Json.Serialization;
using CacheManager.Redis.Extensions;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRedisCacheManager(builder.Configuration.GetConnectionString("Redis"));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();


app.Run();

// Initialize with some options
// builder.Services.AddRedisCacheManager("redis connection string", options =>
// {
//     options.InstanceName = "Library:"; // add this at start of every key in the database
//     options.SerializerOptions = new JsonSerializerOptions
//     {
//         PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
//         Converters =
//         {
//             new JsonStringEnumConverter()
//         }
//     };
//     options.DefaultCacheOptions = new DistributedCacheEntryOptions
//     {
//         SlidingExpiration = TimeSpan.FromDays(1)
//     };
//     
// });
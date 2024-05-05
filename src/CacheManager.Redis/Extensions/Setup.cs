using System;
using CacheManager.Redis.Interfaces;
using CacheManager.Redis.Services;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CacheManager.Redis.Extensions
{
    public static class Setup
    {
        public static IServiceCollection AddRedisCacheManager(this IServiceCollection services, string connectionString, Action<RedisCacheMangerOptions>? options = null)
        {
            var cacheMangerOptions = new RedisCacheMangerOptions();
            options?.Invoke(cacheMangerOptions);
            services.AddSingleton<IRedisDistributedCache>(x =>
            {
                var cacheOptions = x.GetRequiredService<IOptions<RedisCacheOptions>>();
                cacheOptions.Value.Configuration = connectionString;
                if (!string.IsNullOrWhiteSpace(cacheMangerOptions.InstanceName))
                    cacheOptions.Value.InstanceName = cacheMangerOptions.InstanceName;
                
                return new RedisDistributedCache(cacheOptions, cacheMangerOptions.SerializerOptions, cacheMangerOptions.DefaultCacheOptions);
            });
            if (cacheMangerOptions.CustomImplementation is not null &&
                typeof(IRedisCacheManager<>).IsAssignableFrom(cacheMangerOptions.CustomImplementation))
                services.AddScoped(typeof(IRedisCacheManager<>), cacheMangerOptions.CustomImplementation);
            else
                services.AddScoped(typeof(IRedisCacheManager<>), typeof(RedisCacheManager<>));

            return services;
        }
    }
}
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
        public static IServiceCollection AddRedisCacheManager(
            this IServiceCollection services,
            string connectionString,
            Action<RedisCacheMangerOptions>? configure = null)
        {
            var cacheManagerOptions = new RedisCacheMangerOptions();
            configure?.Invoke(cacheManagerOptions);

            services.AddStackExchangeRedisCache(redisOptions =>
            {
                redisOptions.Configuration = connectionString;

                if (!string.IsNullOrWhiteSpace(cacheManagerOptions.InstanceName))
                {
                    redisOptions.InstanceName = cacheManagerOptions.InstanceName;
                }
            });

            services.AddSingleton<IRedisDistributedCache>(provider =>
            {
                var cacheOptions = provider.GetRequiredService<IOptions<RedisCacheOptions>>();
                return new RedisDistributedCache(
                    cacheOptions,
                    cacheManagerOptions.SerializerOptions,
                    cacheManagerOptions.DefaultCacheOptions);
            });

            if (cacheManagerOptions.CustomImplementation is not null &&
                cacheManagerOptions.CustomImplementation.IsAssignableToGenericType(typeof(IRedisCacheManager<>)))
            {
                services.AddScoped(typeof(IRedisCacheManager<>), cacheManagerOptions.CustomImplementation);
            }
            else
            {
                services.AddScoped(typeof(IRedisCacheManager<>), typeof(RedisCacheManager<>));
            }

            return services;
        }
    }
}
using System.Text.Json;
using CacheManager.Redis.Interfaces;
using CacheManager.Redis.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CacheManager.Redis.Extensions
{
    public static class Setup
    {
        public static IServiceCollection AddRedisCacheManager(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<IRedisDistributedCache>(x =>
            {
                var options = x.GetRequiredService<IOptions<RedisCacheOptions>>();
                options.Value.Configuration = connectionString;
                return new RedisDistributedCache(options);
            });
            services.AddScoped(typeof(IRedisCacheManager<>), typeof(RedisCacheManager<>));

            return services;
        }
        
        public static IServiceCollection AddRedisCacheManager(this IServiceCollection services, string connectionString,
            string instanceName)
        {
            services.AddSingleton<IRedisDistributedCache>(x =>
            {
                var options = x.GetRequiredService<IOptions<RedisCacheOptions>>();
                options.Value.Configuration = connectionString;
                options.Value.InstanceName = instanceName;
                return new RedisDistributedCache(options);
            });
            services.AddScoped(typeof(IRedisCacheManager<>), typeof(RedisCacheManager<>));

            return services;
        }
        
        public static IServiceCollection AddRedisCacheManager(this IServiceCollection services, string connectionString, JsonSerializerOptions serializerOptions)
        {
            services.AddSingleton<IRedisDistributedCache>(x =>
            {
                var options = x.GetRequiredService<IOptions<RedisCacheOptions>>();
                options.Value.Configuration = connectionString;
                return new RedisDistributedCache(options, serializerOptions);
            });
            services.AddScoped(typeof(IRedisCacheManager<>), typeof(RedisCacheManager<>));

            return services;
        }
        
        public static IServiceCollection AddRedisCacheManager(this IServiceCollection services, string connectionString,
            DistributedCacheEntryOptions defaultOptions)
        {
            services.AddSingleton<IRedisDistributedCache>(x =>
            {
                var options = x.GetRequiredService<IOptions<RedisCacheOptions>>();
                options.Value.Configuration = connectionString;
                return new RedisDistributedCache(options, defaultOptions);
            });
            services.AddScoped(typeof(IRedisCacheManager<>), typeof(RedisCacheManager<>));

            return services;
        }
        
        public static IServiceCollection AddRedisCacheManager(this IServiceCollection services, string connectionString,
            string instanceName, JsonSerializerOptions serializerOptions)
        {
            services.AddSingleton<IRedisDistributedCache>(x =>
            {
                var options = x.GetRequiredService<IOptions<RedisCacheOptions>>();
                options.Value.Configuration = connectionString;
                options.Value.InstanceName = instanceName;
                return new RedisDistributedCache(options, serializerOptions);
            });
            services.AddScoped(typeof(IRedisCacheManager<>), typeof(RedisCacheManager<>));

            return services;
        }
        
        public static IServiceCollection AddRedisCacheManager(this IServiceCollection services, string connectionString,
            string instanceName, DistributedCacheEntryOptions defaultOptions)
        {
            services.AddSingleton<IRedisDistributedCache>(x =>
            {
                var options = x.GetRequiredService<IOptions<RedisCacheOptions>>();
                options.Value.Configuration = connectionString;
                options.Value.InstanceName = instanceName;
                return new RedisDistributedCache(options, defaultOptions);
            });
            services.AddScoped(typeof(IRedisCacheManager<>), typeof(RedisCacheManager<>));

            return services;
        }
        
        public static IServiceCollection AddRedisCacheManager(this IServiceCollection services, string connectionString,
            DistributedCacheEntryOptions defaultOptions, JsonSerializerOptions serializerOptions)
        {
            services.AddSingleton<IRedisDistributedCache>(x =>
            {
                var options = x.GetRequiredService<IOptions<RedisCacheOptions>>();
                options.Value.Configuration = connectionString;
                return new RedisDistributedCache(options, defaultOptions, serializerOptions);
            });
            services.AddScoped(typeof(IRedisCacheManager<>), typeof(RedisCacheManager<>));

            return services;
        }
        
        public static IServiceCollection AddRedisCacheManager(this IServiceCollection services, string connectionString,
            string instanceName, DistributedCacheEntryOptions defaultOptions, JsonSerializerOptions serializerOptions)
        {
            services.AddSingleton<IRedisDistributedCache>(x =>
            {
                var options = x.GetRequiredService<IOptions<RedisCacheOptions>>();
                options.Value.Configuration = connectionString;
                options.Value.InstanceName = instanceName;
                return new RedisDistributedCache(options, defaultOptions, serializerOptions);
            });
            services.AddScoped(typeof(IRedisCacheManager<>), typeof(RedisCacheManager<>));

            return services;
        }
    }
}
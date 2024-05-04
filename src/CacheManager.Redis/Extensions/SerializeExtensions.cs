using System;
using System.Text.Json;

namespace CacheManager.Redis.Extensions
{
    public static class SerializeExtensions
    {
        public static bool TryDeserialize<TType>(this string value, out TType? response, JsonSerializerOptions? options)
        {
            response = default;
            try
            {
                response = JsonSerializer.Deserialize<TType>(value, options);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
namespace CacheManager.Redis.Extensions
{
    internal static class Helpers
    {
        public static bool HasValue(this string? str) => !string.IsNullOrWhiteSpace(str);
    }
}
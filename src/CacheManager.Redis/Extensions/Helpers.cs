namespace CacheManager.Redis.Extensions
{
    public static class Helpers
    {
        public static bool HasValue(this string? str) => !string.IsNullOrWhiteSpace(str);
    }
}
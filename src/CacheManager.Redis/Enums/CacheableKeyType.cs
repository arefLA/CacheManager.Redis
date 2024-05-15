namespace CacheManager.Redis.Enums;

public enum CacheableKeyType
{
    FromRouteOrQuery,
    FromModel,
    FromProvidedValue,
    MethodName
}
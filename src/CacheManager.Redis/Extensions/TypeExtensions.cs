using System;
using System.Linq;

namespace CacheManager.Redis.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsAssignableToGenericType(this Type? givenType, Type genericType)
        {
            if (givenType is null)
            {
                return false;
            }

            var interfaceTypes = givenType.GetInterfaces();

            if (interfaceTypes.Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType))
                return true;

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            var baseType = givenType.BaseType;
            return baseType != null && baseType.IsAssignableToGenericType(genericType);
        }
    }
}
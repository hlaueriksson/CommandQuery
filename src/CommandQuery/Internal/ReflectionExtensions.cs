using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandQuery.Internal
{
    internal static class ReflectionExtensions
    {
        internal static IEnumerable<Type> GetTypesAssignableTo(this Assembly assembly, Type baseType)
        {
            return assembly.GetTypes()
                .Where(type => type.GetTypeInfo().IsClass && (baseType.IsAssignableFrom(type) || IsAssignableToGenericType(type, baseType)))
                .ToList();
        }

        internal static Type GetInterfaceType(this Type type, Type genericType)
        {
            return type.GetInterfaces().FirstOrDefault(it => it.GetTypeInfo().IsGenericType && it.GetGenericTypeDefinition() == genericType);
        }

        private static bool IsAssignableToGenericType(this Type type, Type genericType)
        {
            return type.GetInterfaces().Any(it => it.GetTypeInfo().IsGenericType && it.GetGenericTypeDefinition() == genericType)
                   || type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == genericType
                   || type.GetTypeInfo().BaseType != null && type.GetTypeInfo().BaseType.IsAssignableToGenericType(genericType);
        }
    }
}
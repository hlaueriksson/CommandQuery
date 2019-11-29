using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandQuery.AspNetCore.Internal
{
    internal static class ReflectionExtensions
    {
        internal static IEnumerable<Type> GetTypesAssignableTo(this Assembly[] assemblies, Type baseType)
        {
            return assemblies.SelectMany(x => x.ExportedTypes)
                .Where(type => type.GetTypeInfo().IsClass && (baseType.IsAssignableFrom(type) || IsAssignableToGenericType(type, baseType)))
                .ToList();
        }

        internal static bool IsAssignableToGenericType(this Type type, Type genericType)
        {
            return type.GetInterfaces().Any(it => it.GetTypeInfo().IsGenericType && it.GetGenericTypeDefinition() == genericType)
                   || (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == genericType)
                   || (type.GetTypeInfo().BaseType != null && type.GetTypeInfo().BaseType.IsAssignableToGenericType(genericType));
        }

        internal static Type GetResultType(this Type type, Type baseType)
        {
            return type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == baseType)
                .Select(i => i.GetGenericArguments()[0])
                .FirstOrDefault();
        }
    }
}
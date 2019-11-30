using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandQuery.Internal
{
    internal static class ReflectionExtensions
    {
        internal static IEnumerable<Type> GetTypesAssignableTo(this Assembly[] assemblies, Type baseType)
        {
            return assemblies.SelectMany(x => x.GetTypesAssignableTo(baseType)).ToList();
        }

        internal static IEnumerable<Type> GetTypesAssignableTo(this Assembly assembly, Type baseType)
        {
            return assembly.ExportedTypes.Where(type => type.IsAssignableTo(baseType)).ToList();
        }

        internal static bool IsAssignableTo(this Type type, Type baseType)
        {
            return type.IsClass && (baseType.IsAssignableFrom(type) || IsAssignableToGenericType(type, baseType));
        }

        internal static Type GetResultType(this Type type, Type baseType)
        {
            return type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == baseType)
                .Select(i => i.GetGenericArguments()[0])
                .FirstOrDefault();
        }

        internal static Type GetHandlerInterface(this Type type, Type genericType)
        {
            return type.GetInterfaces().FirstOrDefault(it => it.GetTypeInfo().IsGenericType && it.GetGenericTypeDefinition() == genericType);
        }

        private static bool IsAssignableToGenericType(this Type type, Type genericType)
        {
            return type.GetInterfaces().Any(it => it.GetTypeInfo().IsGenericType && it.GetGenericTypeDefinition() == genericType)
                   || (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == genericType)
                   || (type.GetTypeInfo().BaseType != null && type.GetTypeInfo().BaseType.IsAssignableToGenericType(genericType));
        }
    }
}
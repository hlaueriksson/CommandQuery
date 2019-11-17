using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("CommandQuery.Tests")]

namespace CommandQuery.DependencyInjection
{
    internal static class ReflectionExtensions
    {
        internal static IEnumerable<Type> GetHandlers(this Assembly assembly, Type genericType)
        {
            return assembly.GetTypes().Where(type => type.GetTypeInfo().IsClass && type.IsAssignableToGenericType(genericType)).ToList();
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
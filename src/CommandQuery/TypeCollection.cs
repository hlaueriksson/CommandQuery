using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandQuery
{
    public interface ITypeCollection
    {
        Type GetType(string key);

        IEnumerable<Type> GetTypes();
    }

    public interface ICommandTypeCollection : ITypeCollection
    {
    }

    public interface IQueryTypeCollection : ITypeCollection
    {
    }

    public abstract class TypeCollection : ITypeCollection
    {
        private readonly Type _genericType;
        private IDictionary<string, Type> _types;

        protected TypeCollection(Type genericType, params Assembly[] assemblies)
        {
            _genericType = genericType;
            LoadTypeCaches(assemblies);
        }

        public Type GetType(string key)
        {
            return _types.ContainsKey(key) ? _types[key] : null;
        }

        public IEnumerable<Type> GetTypes()
        {
            return _types.Values;
        }

        private void LoadTypeCaches(params Assembly[] assemblies)
        {
            var types = GetExportedTypeFor(assemblies)
                .Where(t => t.GetInterfaces().Any(i => i.Name == _genericType.Name))
                .ToArray();

            _types = types.ToDictionary(t => t.Name);
        }

        private static IEnumerable<Type> GetExportedTypeFor(params Assembly[] assemblies)
        {
            return assemblies.SelectMany(a => a.ExportedTypes);
        }
    }

    public class CommandTypeCollection : TypeCollection, ICommandTypeCollection
    {
        public CommandTypeCollection(params Assembly[] assemblies) : base(typeof(ICommand), assemblies)
        {
        }
    }

    public class QueryTypeCollection : TypeCollection, IQueryTypeCollection
    {
        public QueryTypeCollection(params Assembly[] assemblies) : base(typeof(IQuery<>), assemblies)
        {
        }
    }
}
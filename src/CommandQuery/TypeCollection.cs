using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandQuery
{
    /// <summary>
    /// A collection of supported command or query types.
    /// </summary>
    public interface ITypeCollection
    {
        /// <summary>
        /// Returns the type of command or query.
        /// </summary>
        /// <param name="key">The type key</param>
        /// <returns>The type of command or query</returns>
        Type GetType(string key);

        /// <summary>
        /// Returns the types of supported commands or queries.
        /// </summary>
        /// <returns>Supported commands or queries</returns>
        IEnumerable<Type> GetTypes();
    }

    /// <summary>
    /// A collection of supported command types.
    /// </summary>
    public interface ICommandTypeCollection : ITypeCollection
    {
    }

    /// <summary>
    /// A collection of supported query types.
    /// </summary>
    public interface IQueryTypeCollection : ITypeCollection
    {
    }

    /// <summary>
    /// A collection of supported command or query types.
    /// </summary>
    public abstract class TypeCollection : ITypeCollection
    {
        private readonly Type _genericType;
        private IDictionary<string, Type> _types;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeCollection" /> class.
        /// </summary>
        /// <param name="genericType">The base type for commands or queries</param>
        /// <param name="assemblies">The assemblies with commands or queries to support</param>
        protected TypeCollection(Type genericType, params Assembly[] assemblies)
        {
            _genericType = genericType;
            LoadTypeCaches(assemblies);
        }

        /// <summary>
        /// Returns the type of command or query.
        /// </summary>
        /// <param name="key">The type key</param>
        /// <returns>The type of command or query</returns>
        public Type GetType(string key)
        {
            return _types.ContainsKey(key) ? _types[key] : null;
        }

        /// <summary>
        /// Returns the types of supported commands or queries.
        /// </summary>
        /// <returns>Supported commands or queries</returns>
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

    /// <summary>
    /// A collection of supported command types.
    /// </summary>
    public class CommandTypeCollection : TypeCollection, ICommandTypeCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandTypeCollection" /> class.
        /// </summary>
        /// <param name="assemblies">The assemblies with commands to support</param>
        public CommandTypeCollection(params Assembly[] assemblies) : base(typeof(ICommand), assemblies)
        {
        }
    }

    /// <summary>
    /// A collection of supported query types.
    /// </summary>
    public class QueryTypeCollection : TypeCollection, IQueryTypeCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandTypeCollection" /> class.
        /// </summary>
        /// <param name="assemblies">The assemblies with queries to support</param>
        public QueryTypeCollection(params Assembly[] assemblies) : base(typeof(IQuery<>), assemblies)
        {
        }
    }
}
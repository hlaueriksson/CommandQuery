using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandQuery.Internal;

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
        private readonly Type[] _baseTypes;
        private Dictionary<string, Type> _types;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeCollection" /> class.
        /// </summary>
        /// <param name="baseType">The base type for commands or queries</param>
        /// <param name="assemblies">The assemblies with commands or queries to support</param>
        protected TypeCollection(Type baseType, params Assembly[] assemblies)
        {
            _baseTypes = new[] { baseType };
            LoadTypeCaches(assemblies);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeCollection" /> class.
        /// </summary>
        /// <param name="baseTypes">The base types for commands or queries</param>
        /// <param name="assemblies">The assemblies with commands or queries to support</param>
        protected TypeCollection(Type[] baseTypes, params Assembly[] assemblies)
        {
            _baseTypes = baseTypes;
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
            _types = new Dictionary<string, Type>();

            foreach (var baseType in _baseTypes)
            {
                var types = assemblies.SelectMany(assembly => assembly.GetTypesAssignableTo(baseType)).ToList();

                foreach (var type in types)
                {
                    _types.Add(type.Name, type);
                }
            }
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
        public CommandTypeCollection(params Assembly[] assemblies) : base(new[] { typeof(ICommand), typeof(ICommand<>) }, assemblies)
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
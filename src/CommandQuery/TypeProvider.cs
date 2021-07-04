using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandQuery.Internal;

namespace CommandQuery
{
    /// <summary>
    /// A provider of supported command or query types.
    /// </summary>
    public abstract class TypeProvider
    {
        private readonly Type[] _baseTypes;
        private readonly Dictionary<string, Type> _types;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeProvider"/> class.
        /// </summary>
        /// <param name="baseType">The base type for commands or queries.</param>
        /// <param name="assemblies">The assemblies with commands or queries to support.</param>
        protected TypeProvider(Type baseType, params Assembly[] assemblies)
        {
            _baseTypes = new[] { baseType };
            _types = new Dictionary<string, Type>();
            LoadTypeCaches(assemblies);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeProvider"/> class.
        /// </summary>
        /// <param name="baseTypes">The base types for commands or queries.</param>
        /// <param name="assemblies">The assemblies with commands or queries to support.</param>
        protected TypeProvider(Type[] baseTypes, params Assembly[] assemblies)
        {
            _baseTypes = baseTypes;
            _types = new Dictionary<string, Type>();
            LoadTypeCaches(assemblies);
        }

        /// <summary>
        /// Returns the type of command or query.
        /// </summary>
        /// <param name="key">The type key.</param>
        /// <returns>The type of command or query.</returns>
        protected Type? GetType(string key)
        {
            return _types.ContainsKey(key) ? _types[key] : null;
        }

        /// <summary>
        /// Returns the types of supported commands or queries.
        /// </summary>
        /// <returns>Supported commands or queries.</returns>
        protected IReadOnlyList<Type> GetTypes()
        {
            return _types.Values.ToList().AsReadOnly();
        }

        private void LoadTypeCaches(params Assembly[] assemblies)
        {
            foreach (var baseType in _baseTypes)
            {
                foreach (var type in assemblies.SelectMany(assembly => assembly.GetTypesAssignableTo(baseType)).ToList())
                {
                    _types.Add(type.Name, type);
                }
            }
        }
    }
}

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
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public abstract class TypeCollection : ITypeCollection
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    {
        private readonly Type[] _baseTypes;
        private Dictionary<string, Type> _types;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeCollection"/> class.
        /// </summary>
        /// <param name="baseType">The base type for commands or queries.</param>
        /// <param name="assemblies">The assemblies with commands or queries to support.</param>
        protected TypeCollection(Type baseType, params Assembly[] assemblies)
        {
            _baseTypes = new[] { baseType };
            LoadTypeCaches(assemblies);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeCollection"/> class.
        /// </summary>
        /// <param name="baseTypes">The base types for commands or queries.</param>
        /// <param name="assemblies">The assemblies with commands or queries to support.</param>
        protected TypeCollection(Type[] baseTypes, params Assembly[] assemblies)
        {
            _baseTypes = baseTypes;
            LoadTypeCaches(assemblies);
        }

        /// <summary>
        /// Returns the type of command or query.
        /// </summary>
        /// <param name="key">The type key.</param>
        /// <returns>The type of command or query.</returns>
        public Type GetType(string key)
        {
            return _types.ContainsKey(key) ? _types[key] : null;
        }

        /// <summary>
        /// Returns the types of supported commands or queries.
        /// </summary>
        /// <returns>Supported commands or queries.</returns>
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
}

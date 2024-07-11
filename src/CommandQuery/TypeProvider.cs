using System.Reflection;
using CommandQuery.Exceptions;

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
        /// <param name="baseTypes">The base types for commands or queries.</param>
        /// <param name="assemblies">The assemblies with commands or queries to support.</param>
        protected TypeProvider(Type[] baseTypes, params Assembly[] assemblies)
        {
            _baseTypes = baseTypes;
            _types = [];
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

        /// <summary>
        /// Returns exception for when multiple types with the same <see cref="MemberInfo.Name"/> were found.
        /// </summary>
        /// <param name="first">The first type.</param>
        /// <param name="second">The second type.</param>
        /// <returns><see cref="CommandTypeException"/> or <see cref="QueryTypeException"/>.</returns>
        protected abstract Exception GetTypeException(Type first, Type second);

        private void LoadTypeCaches(params Assembly[] assemblies)
        {
            foreach (var baseType in _baseTypes)
            {
                foreach (var type in assemblies.SelectMany(assembly => assembly.GetTypesAssignableTo(baseType)).ToList())
                {
                    var key = type.Name;

                    if (_types.ContainsKey(key))
                    {
                        throw GetTypeException(_types[key], type);
                    }

                    _types.Add(key, type);
                }
            }
        }
    }
}

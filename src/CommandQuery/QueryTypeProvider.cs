using System;
using System.Collections.Generic;
using System.Reflection;

namespace CommandQuery
{
    /// <summary>
    /// A provider of supported query types.
    /// </summary>
    public class QueryTypeProvider : TypeProvider, IQueryTypeProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryTypeProvider"/> class.
        /// </summary>
        /// <param name="assemblies">The assemblies with queries to support.</param>
        public QueryTypeProvider(params Assembly[] assemblies) : base(typeof(IQuery<>), assemblies)
        {
        }

        /// <inheritdoc />
        public Type? GetQueryType(string key) => GetType(key);

        /// <inheritdoc />
        public IReadOnlyList<Type> GetQueryTypes() => GetTypes();
    }
}

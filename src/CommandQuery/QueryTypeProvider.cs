using System;
using System.Collections.Generic;
using System.Reflection;
using CommandQuery.Exceptions;

namespace CommandQuery
{
    /// <inheritdoc cref="IQueryTypeProvider" />
    public class QueryTypeProvider : TypeProvider, IQueryTypeProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryTypeProvider"/> class.
        /// </summary>
        /// <param name="assemblies">The assemblies with queries to support.</param>
        /// <exception cref="QueryTypeException">Multiple queries with the same name was found.</exception>
        public QueryTypeProvider(params Assembly[] assemblies)
            : base(new[] { typeof(IQuery<>) }, assemblies)
        {
        }

        /// <inheritdoc />
        public Type? GetQueryType(string key) => GetType(key);

        /// <inheritdoc />
        public IReadOnlyList<Type> GetQueryTypes() => GetTypes();

        /// <inheritdoc />
        protected override Exception GetTypeException(Type first, Type second) =>
            new QueryTypeException($"Multiple queries with the same name was found: '{first?.AssemblyQualifiedName}', '{second?.AssemblyQualifiedName}'");
    }
}

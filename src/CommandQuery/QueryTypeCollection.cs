using System.Reflection;

namespace CommandQuery
{
    /// <summary>
    /// A collection of supported query types.
    /// </summary>
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public class QueryTypeCollection : TypeCollection, IQueryTypeCollection
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryTypeCollection"/> class.
        /// </summary>
        /// <param name="assemblies">The assemblies with queries to support.</param>
        public QueryTypeCollection(params Assembly[] assemblies) : base(typeof(IQuery<>), assemblies)
        {
        }
    }
}

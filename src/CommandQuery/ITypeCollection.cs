using System;
using System.Collections.Generic;

namespace CommandQuery
{
    /// <summary>
    /// A collection of supported command or query types.
    /// </summary>
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public interface ITypeCollection // TODO: delete
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    {
        /// <summary>
        /// Returns the type of command or query.
        /// </summary>
        /// <param name="key">The type key.</param>
        /// <returns>The type of command or query.</returns>
#pragma warning disable CA1716 // Identifiers should not match keywords
        Type GetType(string key);
#pragma warning restore CA1716 // Identifiers should not match keywords

        /// <summary>
        /// Returns the types of supported commands or queries.
        /// </summary>
        /// <returns>Supported commands or queries.</returns>
        IEnumerable<Type> GetTypes();
    }

    /// <summary>
    /// A collection of supported command types.
    /// </summary>
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public interface ICommandTypeCollection : ITypeCollection
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    {
    }

    /// <summary>
    /// A collection of supported query types.
    /// </summary>
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public interface IQueryTypeCollection : ITypeCollection
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    {
    }
}

using System;
using System.Collections.Generic;

namespace CommandQuery
{
    /// <summary>
    /// A provider of supported command types.
    /// </summary>
    public interface ICommandTypeProvider
    {
        /// <summary>
        /// Returns the type of command.
        /// </summary>
        /// <param name="key">The type key.</param>
        /// <returns>The type of command.</returns>
        Type? GetCommandType(string key);

        /// <summary>
        /// Returns the types of supported commands.
        /// </summary>
        /// <returns>Supported commands.</returns>
        IReadOnlyList<Type> GetCommandTypes();
    }
}

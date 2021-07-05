using System;
using System.Collections.Generic;
using System.Reflection;

namespace CommandQuery
{
    /// <inheritdoc cref="ICommandTypeProvider" />
    public class CommandTypeProvider : TypeProvider, ICommandTypeProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandTypeProvider"/> class.
        /// </summary>
        /// <param name="assemblies">The assemblies with commands to support.</param>
        public CommandTypeProvider(params Assembly[] assemblies) : base(new[] { typeof(ICommand), typeof(ICommand<>) }, assemblies)
        {
        }

        /// <inheritdoc />
        public Type? GetCommandType(string key) => GetType(key);

        /// <inheritdoc />
        public IReadOnlyList<Type> GetCommandTypes() => GetTypes();
    }
}

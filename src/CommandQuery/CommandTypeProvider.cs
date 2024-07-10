using System.Reflection;
using CommandQuery.Exceptions;

namespace CommandQuery
{
    /// <inheritdoc cref="ICommandTypeProvider" />
    public class CommandTypeProvider : TypeProvider, ICommandTypeProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandTypeProvider"/> class.
        /// </summary>
        /// <param name="assemblies">The assemblies with commands to support.</param>
        /// <exception cref="CommandTypeException">Multiple commands with the same name was found.</exception>
        public CommandTypeProvider(params Assembly[] assemblies)
            : base(new[] { typeof(ICommand), typeof(ICommand<>) }, assemblies)
        {
        }

        /// <inheritdoc />
        public Type? GetCommandType(string key) => GetType(key);

        /// <inheritdoc />
        public IReadOnlyList<Type> GetCommandTypes() => GetTypes();

        /// <inheritdoc />
        protected override Exception GetTypeException(Type first, Type second) =>
            new CommandTypeException($"Multiple commands with the same name was found: '{first?.AssemblyQualifiedName}', '{second?.AssemblyQualifiedName}'");
    }
}

using System.Reflection;

namespace CommandQuery
{
    /// <summary>
    /// A collection of supported command types.
    /// </summary>
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public class CommandTypeCollection : TypeCollection, ICommandTypeCollection
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandTypeCollection"/> class.
        /// </summary>
        /// <param name="assemblies">The assemblies with commands to support.</param>
        public CommandTypeCollection(params Assembly[] assemblies) : base(new[] { typeof(ICommand), typeof(ICommand<>) }, assemblies)
        {
        }
    }
}

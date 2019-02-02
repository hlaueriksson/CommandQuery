using System.Reflection;
using CommandQuery.DependencyInjection;
using Machine.Specifications;

namespace CommandQuery.Specs.DependencyInjection
{
    [Subject(typeof(CommandExtensions))]
    public class CommandExtensionsSpecs
    {
        It should_GetCommandProcessor_from_Assembly = () =>
        {
            var assembly = typeof(FakeCommandHandler).GetTypeInfo().Assembly;
            var result = assembly.GetCommandProcessor();

            result.ShouldNotBeNull();
            result.GetCommands().ShouldContain(typeof(FakeCommand));
        };

        It should_GetCommandProcessor_from_Assemblies = () =>
        {
            var assemblies = new[] { typeof(FakeCommandHandler).GetTypeInfo().Assembly };
            var result = assemblies.GetCommandProcessor();

            result.ShouldNotBeNull();
            result.GetCommands().ShouldContain(typeof(FakeCommand));
        };
    }
}
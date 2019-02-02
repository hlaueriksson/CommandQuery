using System.Reflection;
using CommandQuery.DependencyInjection;
using Machine.Specifications;

namespace CommandQuery.Specs.DependencyInjection
{
    [Subject(typeof(QueryExtensions))]
    public class QueryExtensionsSpecs
    {
        It should_GetQueryProcessor_from_Assembly = () =>
        {
            var assembly = typeof(FakeQueryHandler).GetTypeInfo().Assembly;
            var result = assembly.GetQueryProcessor();

            result.ShouldNotBeNull();
            result.GetQueries().ShouldContain(typeof(FakeQuery));
        };

        It should_GetQueryProcessor_from_Assemblies = () =>
        {
            var assemblies = new[] { typeof(FakeQueryHandler).GetTypeInfo().Assembly };
            var result = assemblies.GetQueryProcessor();

            result.ShouldNotBeNull();
            result.GetQueries().ShouldContain(typeof(FakeQuery));
        };
    }
}
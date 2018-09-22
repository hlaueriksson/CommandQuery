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
            var assembly = typeof(QueryExtensions).GetTypeInfo().Assembly;
            var result = assembly.GetQueryProcessor();

            result.ShouldNotBeNull();
        };

        It should_GetQueryProcessor_from_Assemblies = () =>
        {
            var assemblies = new[] { typeof(QueryExtensions).GetTypeInfo().Assembly };
            var result = assemblies.GetQueryProcessor();

            result.ShouldNotBeNull();
        };
    }
}
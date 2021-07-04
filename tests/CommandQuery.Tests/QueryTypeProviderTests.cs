using System.Reflection;
using FluentAssertions;
using LoFuUnit.NUnit;
using NUnit.Framework;

namespace CommandQuery.Tests
{
    public class QueryTypeProviderTests
    {
        [LoFu, Test]
        public void when_getting_the_type()
        {
            Subject = new QueryTypeProvider(typeof(FakeQuery).GetTypeInfo().Assembly);

            void should_return_the_type_of_query_if_the_query_name_is_found() => Subject.GetQueryType("FakeQuery").Should().NotBeNull();

            void should_return_null_if_the_query_name_is_not_found() => Subject.GetQueryType("NotFoundQuery").Should().BeNull();
        }

        QueryTypeProvider Subject;
    }
}

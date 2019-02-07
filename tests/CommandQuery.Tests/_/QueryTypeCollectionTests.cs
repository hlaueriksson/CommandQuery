using System.Reflection;
using FluentAssertions;
using LoFuUnit.NUnit;
using NUnit.Framework;

namespace CommandQuery.Tests._
{
    public class QueryTypeCollectionTests
    {
        [LoFu, Test]
        public void when_getting_the_type()
        {
            Subject = new QueryTypeCollection(typeof(FakeQuery).GetTypeInfo().Assembly);

            void should_return_the_type_of_query_if_the_query_name_is_found() => Subject.GetType("FakeQuery").Should().NotBeNull();

            void should_return_null_if_the_query_name_is_not_found() => Subject.GetType("NotFoundQuery").Should().BeNull();
        }

        QueryTypeCollection Subject;
    }
}
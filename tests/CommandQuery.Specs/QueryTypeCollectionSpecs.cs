using System.Reflection;
using Machine.Specifications;

namespace CommandQuery.Specs
{
    public class QueryTypeCollectionSpecs
    {
        [Subject(typeof(QueryTypeCollection))]
        public class when_getting_the_type
        {
            Establish context = () => Subject = new QueryTypeCollection(typeof(FakeQuery).GetTypeInfo().Assembly);

            It should_return_the_type_of_query_if_the_query_name_is_found = () => Subject.GetType("FakeQuery").ShouldNotBeNull();

            It should_return_null_if_the_query_name_is_not_found = () => Subject.GetType("NotFoundQuery").ShouldBeNull();

            static QueryTypeCollection Subject;
        }
    }
}
using System.Reflection;
using Machine.Specifications;

namespace CommandQuery.Specs
{
    public class CommandTypeCollectionSpecs
    {
        [Subject(typeof(CommandTypeCollection))]
        public class when_getting_the_type
        {
            Establish context = () => Subject = new CommandTypeCollection(typeof(FakeCommand).GetTypeInfo().Assembly);

            It should_return_the_type_of_command_if_the_command_name_is_found = () => Subject.GetType("FakeCommand").ShouldNotBeNull();

            It should_return_null_if_the_command_name_is_not_found = () => Subject.GetType("NotFoundCommand").ShouldBeNull();

            static CommandTypeCollection Subject;
        }
    }
}
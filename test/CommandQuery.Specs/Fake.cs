namespace CommandQuery.Specs
{
    public class FakeCommand : ICommand
    {
    }

    public class FakeQuery : IQuery<FakeResult>
    {
    }

    public class FakeResult
    {
    }
}
using System.Web.Http.Tracing;

namespace CommandQuery.AspNet.WebApi.Tests
{
    public class FakeCommandController : BaseCommandController
    {
        public FakeCommandController(ICommandProcessor commandProcessor) : base(commandProcessor)
        {
        }

        public FakeCommandController(ICommandProcessor commandProcessor, ITraceWriter logger) : base(commandProcessor, logger)
        {
        }
    }

    public class FakeQueryController : BaseQueryController
    {
        public FakeQueryController(IQueryProcessor queryProcessor) : base(queryProcessor)
        {
        }

        public FakeQueryController(IQueryProcessor queryProcessor, ITraceWriter logger) : base(queryProcessor, logger)
        {
        }
    }
}
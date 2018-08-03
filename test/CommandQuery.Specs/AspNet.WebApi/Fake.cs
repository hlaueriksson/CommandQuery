#if NET461
using System.Web.Http.Tracing;
using CommandQuery.AspNet.WebApi;

namespace CommandQuery.Specs.AspNet.WebApi
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
#endif
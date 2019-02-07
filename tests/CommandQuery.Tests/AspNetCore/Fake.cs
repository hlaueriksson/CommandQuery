#if NETCOREAPP2_0
using CommandQuery.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;

namespace CommandQuery.Tests.AspNetCore
{
    public class FakeCommandController : BaseCommandController
    {
        public FakeCommandController(ICommandProcessor commandProcessor) : base(commandProcessor)
        {
        }

        public FakeCommandController(ICommandProcessor commandProcessor, ILogger<BaseCommandController> logger) : base(commandProcessor, logger)
        {
        }
    }

    public class FakeQueryController : BaseQueryController
    {
        public FakeQueryController(IQueryProcessor queryProcessor) : base(queryProcessor)
        {
        }

        public FakeQueryController(IQueryProcessor queryProcessor, ILogger<BaseQueryController> logger) : base(queryProcessor, logger)
        {
        }
    }

    public static class Fake
    {
        public static ControllerContext ControllerContext(Mock<HttpContext> fakeHttpContext = null, Mock<HttpRequest> fakeHttpRequest = null, Mock<HttpResponse> fakeHttpResponse = null)
        {
            if (fakeHttpContext == null) fakeHttpContext = new Mock<HttpContext>();
            if (fakeHttpRequest == null) fakeHttpRequest = new Mock<HttpRequest>();
            if (fakeHttpResponse == null) fakeHttpResponse = new Mock<HttpResponse>();

            fakeHttpContext.Setup(httpContext => httpContext.Request).Returns(fakeHttpRequest.Object);
            fakeHttpContext.Setup(httpContext => httpContext.Response).Returns(fakeHttpResponse.Object);

            fakeHttpRequest.SetupGet(x => x.Query).Returns(new QueryCollection());

            var actionContext = new ActionContext(fakeHttpContext.Object, new RouteData(), new ControllerActionDescriptor());

            return new ControllerContext(actionContext);
        }
    }
}
#endif
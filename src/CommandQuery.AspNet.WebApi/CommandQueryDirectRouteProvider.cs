using System.Collections.Generic;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace CommandQuery.AspNet.WebApi
{
    /// <summary>
    /// Direct route provider for the <see cref="System.Web.Http.HttpConfiguration" /> with attributes inheritance for actions.
    /// </summary>
    public class CommandQueryDirectRouteProvider : DefaultDirectRouteProvider
    {
        /// <summary>Gets a set of route factories for the given action descriptor.</summary>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <returns>A set of route factories.</returns>
        protected override IReadOnlyList<IDirectRouteFactory> GetActionRouteFactories(HttpActionDescriptor actionDescriptor)
        {
            return actionDescriptor.GetCustomAttributes<IDirectRouteFactory>(inherit: true);
        }
    }
}
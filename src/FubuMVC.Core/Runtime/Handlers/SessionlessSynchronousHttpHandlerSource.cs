using System.Web;
using System.Web.Routing;
using FubuCore.Binding;

namespace FubuMVC.Core.Runtime.Handlers
{
    public class SessionlessSynchronousHttpHandlerSource : IHttpHandlerSource
    {
        public IHttpHandler Build(IBehaviorInvoker invoker, ServiceArguments arguments, RouteValueDictionary routeValues)
        {
            return new SessionLessFubuHttpHandler(invoker, arguments, routeValues);
        }
    }

    public class ReadOnlySessionSynchronousHttpHandlerSource : IHttpHandlerSource
    {
        public IHttpHandler Build(IBehaviorInvoker invoker, ServiceArguments arguments, RouteValueDictionary routeValues)
        {
            return new ReadOnlySessionFubuHttpHandler(invoker, arguments, routeValues);
        }
    }
}
using System.Collections.Generic;
using System.Web;
using FubuCore.Binding;
using System.Web.SessionState;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Runtime.Handlers
{
    public interface IFubuHttpHandler : IHttpHandler
    {
        T Service<T>() where T : class;
    }

    public class SessionLessFubuHttpHandler : IFubuHttpHandler
    {
        private readonly IBehaviorInvoker _invoker;
        private readonly ServiceArguments _arguments;
        private readonly IDictionary<string, object> _routeData;

        public SessionLessFubuHttpHandler(IBehaviorInvoker invoker, ServiceArguments arguments, IDictionary<string, object> routeData)
        {
            _invoker = invoker;
            _arguments = arguments;
            _routeData = routeData;
        }

        public void ProcessRequest(HttpContext context)
        {
            var requestCompletion = new RequestCompletion();
            requestCompletion.Start(() => _invoker.Invoke(_arguments, _routeData, requestCompletion));
        }

        public bool IsReusable { get { return false; } }

        public T Service<T>() where T : class
        {
            return _arguments.Get<T>();
        }
    }

    public class ReadOnlySessionFubuHttpHandler : SessionLessFubuHttpHandler, IReadOnlySessionState 
    {
        public ReadOnlySessionFubuHttpHandler(IBehaviorInvoker invoker, ServiceArguments arguments, IDictionary<string, object> routeData)
            : base(invoker, arguments, routeData)
        {
        }
    }
}
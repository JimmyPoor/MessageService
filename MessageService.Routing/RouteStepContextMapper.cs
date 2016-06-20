using MessageService.Core;
using MessageService.Core.Util;
using System;

namespace MessageService.Routing
{
    public class RouteStepContextMapper : IObjectMapper<ProcessContext, RouteContext>
    {
        public Func<ProcessContext, RouteContext> Mapping => MappingRouteContext;

        protected virtual  RouteContext MappingRouteContext(ProcessContext ctx)
        {
            var routeContext = new RouteContext(ctx);
            var checkResult=RouteContext.Check(routeContext);
            return routeContext;
        }
    }
}

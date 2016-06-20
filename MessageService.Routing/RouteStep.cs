using MessageService.Core.Steps;
using System;
using System.Threading.Tasks;

namespace MessageService.Routing
{
    public class RouteStep : IStep
    {
        public bool IsEnable { get; set; }

        public Guid StepId => Guid.NewGuid();

        public string StepName => typeof(RouteStep).Name;

        public virtual Task ExcuteStep(StepProcessContext ctx)
        {
            var processContext = ctx.InnerContext;
            var mapping = new RouteStepContextMapper().Mapping;
            var routeContext = mapping(ctx.InnerContext);
            var router = processContext.Settings.GetRouter();
            Core.Util.AssistClass.ExceptionWhenNull(router);
            Core.Util.AssistClass.ExceptionWhenNull(routeContext);
            var results = router.Route(routeContext.RouteMessge, routeContext);
            processContext.Settings.Set("RouteResult", results.Result);
            return Task.FromResult(0);
        }
    }


    public class IncomingRouteStep : RouteStep, IncomingStep
    {

    }

    public class OutgoingRouteStep : RouteStep, IOutgoingStep
    {
    }
}

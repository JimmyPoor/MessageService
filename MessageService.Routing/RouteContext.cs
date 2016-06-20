using MessageService.Core;

namespace MessageService.Routing
{
    public class RouteContext
    {
        public RouteContext(ProcessContext processContext)
        {
            Core.Util.AssistClass.ExceptionWhenNull(processContext);
            this.InnerContext = processContext;
        }

       public static bool Check(RouteContext context)
        {
            var processContext = context.InnerContext;
            ProcessContext.Check(processContext);
            return !Core.Util.AssistClass.IsNull(context) &&
                        !Core.Util.AssistClass.IsNull(processContext) &&
                        !Core.Util.AssistClass.IsNull(context.InnerContext.CurrentMessage);
    
        }

        public object RouteMessge => InnerContext.CurrentMessage;
       // public  RouteResult RouteResult { get; set; }
        public ProcessContext InnerContext { get; private set; }
    }
}

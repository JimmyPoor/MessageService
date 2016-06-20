using MessageService.Core.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Core.Middleware.Steps
{
    public class SubscribeMessageStep : IncomingStep
    {
        public bool IsEnable { get; set; }

        public Guid StepId => Guid.NewGuid();

        public string StepName => typeof(SubscribeMessageStep).Name;

        public Task ExcuteStep(StepProcessContext ctx)
        {
            var processCtx = ctx.InnerContext;
            var message = processCtx.CurrentMessage as ISubscribeMessage; // is message is subscribe message means subsribe request

            if (Util.AssistClass.IsNull(message))
            {
                // ctx.Errors.Add(new StepError())
                return Task.FromResult(0);
            }
            var subTarget = message.SubscribeTarget;
            var endpoint = processCtx.CurrentEndpoint;
            endpoint.Subscribe(subTarget);
            return Task.FromResult(0);
        }
    }
}

using MessageService.Core.Util;
using System;

namespace MessageService.Core.Steps
{
    public class StepContextMapper: IObjectMapper<ProcessContext,StepProcessContext>
    {

        public StepContextMapper(Action<StepProcessContext> callback =null)
        {
            this._callback = callback;
        }
        Action<StepProcessContext> _callback;
        public Func<ProcessContext, StepProcessContext> Mapping => pctx => MappingFromProcessContextToStepContext(pctx);

        protected virtual StepProcessContext MappingFromProcessContextToStepContext(ProcessContext pctx)
        {
            Util.AssistClass.ExceptionWhenNull(pctx);
            var stepContext= new StepProcessContext(pctx);
            if(_callback!=null)
            {
                _callback(stepContext);
            }
            return stepContext;
        }
    }
}

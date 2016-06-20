using System.Collections.Generic;

namespace MessageService.Core.Steps
{
    public class StepProcessContext
    {
        public StepProcessContext(ProcessContext ctx)
        {
            Util.AssistClass.ExceptionWhenNull(ctx);
            this.InnerContext = ctx;
            this.Errors = new List<StepError>();
        }

        public List<StepError> Errors { get; set; }
        public ProcessContext InnerContext { get; private set; }
    }

}

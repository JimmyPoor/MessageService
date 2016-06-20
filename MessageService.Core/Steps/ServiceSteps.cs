using MessageService.Core.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessageService.Core.Steps
{
    /// <summary>
    /// this class for operation for steps registration and invoke action scenario
    /// </summary>
    public class ServiceSteps
    {
        ConcurrentQueue<IStep> _steps = new ConcurrentQueue<IStep>();
        public async virtual Task<StepProcessContext> StartSteps(StepProcessContext context)
        {
            Util.AssistClass.ExceptionWhenNull(context);
            IStep step = null;
            while (_steps.TryDequeue(out step))
            {
                try {
                    await step.ExcuteStep(context);
                }
                catch(Exception e)
                {
                    var stepError = new StepError(e, StaticStringDefinition.STEP_ERROR, step);
                    context.Errors.Add(stepError);
                }
            }
            return context;
        }

        public virtual void RegistStep(IStep step)
        {
            Util.AssistClass.ExceptionWhenNull(step);
            _steps.Enqueue(step);
        }

        public virtual void RegistSteps(IEnumerable<IStep> steps)
        {
            Util.AssistClass.ExceptionWhenNull(steps);
            foreach(var step in steps)
            {
                RegistStep(step);
            }
        }

        public int Count => _steps.Count;

        public static ServiceSteps Create()
        {
            return new ServiceSteps();
        }
    }
}

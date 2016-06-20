using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MessageService.Core.Steps
{
    public static class StepsManager
    {
        public static IEnumerable<IOutgoingStep> OutgoingSteps { get;  set; } // send, publish steps
        public static IEnumerable<IncomingStep> IncomingSteps { get;  set; } // receive ,invoke steps

        public static void RegistSteps(ServiceSteps steps, IEnumerable<IStep> needRegistSteps)
        {
            if (Util.AssistClass.IsNull(needRegistSteps)) return;
            foreach (var step in needRegistSteps)
            {
                steps.RegistStep(step);
            }
        }

        public  static void BindIncomingSteps(params Assembly[] assemblies)
        {
            if (Util.AssistClass.IsNull(IncomingSteps))
            {
                IncomingSteps = FindStepsFromAssemblies<IncomingStep>(assemblies);
            }
        }

        public static void BindOutgoingSteps(params Assembly[] assemblies)
        {
            if (Util.AssistClass.IsNull(OutgoingSteps))
            {
                OutgoingSteps = FindStepsFromAssemblies<IOutgoingStep>(assemblies);
            }
        }
        public static IEnumerable<Step> FindStepsFromAssemblies<Step>(params Assembly[] assemblies) where Step : IStep
        {
            if (Util.AssistClass.IsNull(assemblies)) return null;
            List<Step> steps = new List<Step>();
            foreach (var assemble in assemblies)
            {
                var searchStep = assemble.GetTypes()
                    .Where(type => typeof(Step).IsAssignableFrom(type) && type.IsClass)
                    .Select(stepType => (Step)Activator.CreateInstance(stepType));
                steps.AddRange(searchStep);
            }
            return steps;
        }

        public static async Task<StepProcessContext> InvokeSteps(ProcessContext ctx, IEnumerable<IStep> steplist)
        {
            var steps = ServiceSteps.Create();
            var mappingFuc = new StepContextMapper().Mapping;
            RegistSteps(steps, steplist);
            return await InvokeSteps(steps, ctx, mappingFuc);
        }

        public static async Task<StepProcessContext> InvokeSteps(ServiceSteps steps, ProcessContext ctx, Func<ProcessContext, StepProcessContext> mapper)
        {
            Util.AssistClass.ExceptionWhenNull(mapper);
            var stepCtx = mapper(ctx);
            return await steps.StartSteps(stepCtx);
        }

        public static Step GetStepFromCache<Step>(string stepName, IEnumerable<Step> cache) where Step : IStep
        {
            if (Util.AssistClass.IsNull(cache) || cache.Count() <= 0) return default(Step);

            return cache.FirstOrDefault(step => step.StepName == stepName);
        }

        public static void FindStepsByNamesFromCacheAndAddToList<Step>(IList<Step> steps,string[] stepNames, IEnumerable<Step> cache) where Step :IStep
        {
            Util.AssistClass.ExceptionWhenNull(steps);
            foreach (var stepName in stepNames)
            {
                var step = GetStepFromCache(stepName, cache);
                if (!steps.Contains(step))
                {
                    steps.Add(step);
                }
            }
        }
    }
}

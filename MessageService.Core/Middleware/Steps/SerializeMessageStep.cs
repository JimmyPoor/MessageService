using MessageService.Core.Message;
using MessageService.Core.Steps;
using System;
using System.Threading.Tasks;

namespace MessageService.Core.Middleware.Steps
{
    public sealed class SerializeMessageStep : IOutgoingStep
    {
        public bool IsEnable { get;  set; }

        public Guid StepId => Guid.NewGuid();

        public string StepName => typeof(SerializeMessageStep).Name;

        public async  Task ExcuteStep(StepProcessContext ctx)
        {
            Util.AssistClass.ExceptionWhenNull(ctx);
            var processContext = ctx.InnerContext;
            ProcessContext.Check(processContext);
            var settings = processContext.Settings;
            var message =processContext.CurrentMessage;
            var serializer = settings.GetSerilizerFromSettings();
            var body= await serializer.Searilize(message);

            processContext.CurrentMessage = new ServiceMessage(body, settings);
        }
    }
}

using MessageService.Core.Steps;
using System;
using System.Threading.Tasks;

namespace MessageService.Core.Middleware.Steps
{
    public class DeserializeMessageStep : IncomingStep
    {
        public bool IsEnable { get; set; }

        public Guid StepId => Guid.NewGuid();

        public string StepName => typeof(DeserializeMessageStep).Name;

        public async Task ExcuteStep(StepProcessContext ctx)
        {
            Util.AssistClass.ExceptionWhenNull(ctx);
            var processContext = ctx.InnerContext;
            ProcessContext.Check(processContext);

            var settings = processContext.Settings;
            var serilizer = settings.GetSerilizerFromSettings();
            var body = processContext.CurrentMessage.Body;
            processContext.CurrentMessage = await serilizer.Dsearilize<IServiceMessage>(body) as IServiceMessage;
        }
    }
}

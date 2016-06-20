using System;
using System.Threading.Tasks;

namespace MessageService.Core.Steps
{
    public interface IStep
    {
        Guid StepId { get; }
        string StepName { get; }
        bool IsEnable { get; set; }
        Task ExcuteStep(StepProcessContext ctx);
    }

    public interface IOutgoingStep : IStep { }
    
    public interface IncomingStep : IStep { }

}

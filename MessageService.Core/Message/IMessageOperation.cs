using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessageService.Core.Message
{
    public interface IMessageOperation : IComponent
    {
        Task Send(string target, object msg, ProcessContext ctx, Action<ProcessContext> callback);
        Task SendServiceMessage(string target, IServiceMessage msg, ProcessContext ctx, Action<ProcessContext> callback);
        Task Subscribe(string target, ISubscribeMessage subMessage, ProcessContext ctx, Action<ProcessContext> callback);
        Task Publish(string[] subTargets, object message, ProcessContext ctx, Action<ProcessContext> callback);
        Task Reply(IReplyMessage msg, ProcessContext ctx, Action<ProcessContext> callback);
        event OperationErrorHandler OnErrorWhenMessageOperation;
    }




    public delegate void OperationErrorHandler(object sender, SendErrorEventArgs args);

    public class SendErrorEventArgs : ErrorEventArgs<SendError>
    {

        public SendErrorEventArgs(List<SendError> errors)
            : base(errors)
        {

        }
    }
    public class SendError : Error
    {
        public string OperationType { get; private set; }
        public SendError(Exception innerExcepiton, string error, object innerErrors, string operationType)
            : base(innerExcepiton, error, innerErrors)
        {
            this.OperationType = operationType;
        }
    }
}

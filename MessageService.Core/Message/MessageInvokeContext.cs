using System;
namespace MessageService.Core.Message
{
    public class MessageInvokeContext
    {
        public ProcessContext InnerContext { get; set; }

        public async void Reply(IReplyMessage message, Action<ProcessContext> callback = null)
        {
            Util.AssistClass.ExceptionWhenNull(message);
            Util.AssistClass.ExceptionWhenNull(InnerContext);
            Util.AssistClass.ExceptionWhenNull(InnerContext.Sender);

            var target = InnerContext.ReceiveFromEndpointName;
            message.SetReplyTarget(target);
             await InnerContext.Sender.Reply(message, InnerContext, callback);
        }

        public async void Publish(IServiceMessage message)
        {
            Util.AssistClass.ExceptionWhenNull(message);
            Util.AssistClass.ExceptionWhenNull(InnerContext);
            Util.AssistClass.ExceptionWhenNull(InnerContext.CurrentEndpoint);

            await InnerContext.CurrentEndpoint.Publish(message);
        }

        public async void Send(string target,IServiceMessage message,Action<ProcessContext> callback=null)
        {
            Util.AssistClass.ExceptionWhenNull(message);
            Util.AssistClass.ExceptionWhenNull(InnerContext);
            Util.AssistClass.ExceptionWhenNull(InnerContext.CurrentEndpoint);

            await InnerContext.CurrentEndpoint.Send(target,message, callback);
        }



    }
}

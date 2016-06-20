using System;
using System.Threading.Tasks;

namespace MessageService.Core.Middleware
{
    public interface IMiddleware :IStartableComponet
    {
        void BindSendToEndpoint(string endpoint);
        void BindReceveFromEndpoint(string endpoint);
        IMiddlewareBehavior Behavior { get; }
    }


    public interface IMiddlewareBehavior:IComponent
    {
       Task Send(string queue, byte[] message);
       Task Send(string queue, byte[] message,ProcessContext ctx,Action<ProcessContext> callback); // when invoke outgoing steps after sending message
        Task Listen(string queue,
                           ProcessContext ctx,
                           Action<Task<byte[]>, ProcessContext, Action<ProcessContext>> whenReceive,
                           Action<ProcessContext> callback=null);//when invoke incoming steps after receive message
        Task Close();
    }

}

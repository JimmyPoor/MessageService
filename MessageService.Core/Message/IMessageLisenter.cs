using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessageService.Core.Message
{

    public interface IMessageLisenter : IComponent
    {
        Task Listen(ProcessContext ctx, Action<ProcessContext> callback = null);

        //Task Listen(string target);

        event ReceivingErrorHandler OnErrorWhenReceiving;
    }



    public delegate void ReceivingErrorHandler(object sender,ReceivingErrorEventArgs args);

    public class ReceivingErrorEventArgs : ErrorEventArgs<ReceivingError>
    {
        public ReceivingErrorEventArgs(List<ReceivingError> errors)
            :base(errors)
        {

        }
    }
    public class ReceivingError : Error
    {
        public ReceivingError(Exception innerExcepiton, string error,object innerErrors)
            :base(innerExcepiton, error,innerErrors)
        {
        }
    }
}

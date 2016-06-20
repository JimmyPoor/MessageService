using System.Threading.Tasks;

namespace MessageService.Core.Message
{
    public interface IMessageInvoker<Message>
    {
        Task Invoke(Message msg, MessageInvokeContext ctx);
    }


}

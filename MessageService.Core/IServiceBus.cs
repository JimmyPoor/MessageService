using System.Threading.Tasks;

namespace MessageService.Core
{
    public interface IServiceBus :IStartable
    {
        void Initial(Settings settings);
        Task Send(string target, object message);
    }
}

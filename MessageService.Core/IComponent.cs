using System;
using System.Threading.Tasks;

namespace MessageService.Core
{

    public interface IComponent:IDisposable
    {
        string DefaultName { get; }
        Settings Settings { get; set; }
        Task Setup(Settings settings = null);
        bool IsEnable { get; }
    }

    public interface IStartableComponet:IStartable, IComponent
    {
    }
}

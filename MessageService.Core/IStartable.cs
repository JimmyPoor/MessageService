using System;
using System.Threading.Tasks;

namespace MessageService.Core
{
    public interface IStartable :IDisposable
    {
        Task Start();
        Task Stop();
        bool IsWorking { get; }
    }
}

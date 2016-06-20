using System;
using System.Threading.Tasks;

namespace MessageService.Core.Serializor
{
    public interface ISearilizer:IComponent
    {
        Task<Byte[]> Searilize<T>(T t);
        Task<T> Dsearilize<T>(byte[] byteArray);
    }

    public interface IMessageSerilizer : ISearilizer
    {
    }
}

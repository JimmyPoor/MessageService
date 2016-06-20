using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Core.Persistance
{
    public interface IStorage
    {
        Task<StorageResult> Store<T>(IStorageSource<T> t);
    }

    public interface IStorageSource<T>
    {
        T Data { get; }
        void Bind(T t);
    }   

    public class StorageResult
    {
        public virtual bool IsSuccess { get; private set; }
        public virtual int EffectCount { get; private set; }
        public object Result { get; private set; }
    }
}

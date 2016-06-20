using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Core.Persistance
{
    public  class StorageSource<T> : IStorageSource<T>
    {
        public T Data { get; protected set; }

        public virtual void Bind(T data)
        {
            Util.AssistClass.ExceptionWhenNull(data);
            this.Data = data;
        }
    }
}

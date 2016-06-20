using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Core.Persistance
{
    public class ServicePersistence: IPersistence
    {
        public ServicePersistence() { }
        public virtual Task<StorageResult> Persist<T>(IStorage storage, IStorageSource<T> source)
        {
            Util.AssistClass.ExceptionWhenNull(storage);
            Util.AssistClass.ExceptionWhenNull(source);

            return storage.Store(source);
        }
    }
}

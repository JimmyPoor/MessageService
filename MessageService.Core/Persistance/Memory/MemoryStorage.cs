using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageService.Core.Persistance
{

    public class MemoryStorage : IStorage
    {
        ConcurrentDictionary<Type, object> _caches => new ConcurrentDictionary<Type, object>();

        public Task<StorageResult> Store<T>(IStorageSource<T> source)
        {
            Util.AssistClass.ExceptionWhenNull(source);
            var data = source.Data;
            var type = ConvertSourceDataType(typeof(T));

            return null;
        }

        Type  ConvertSourceDataType(Type  souceDataType)
        {
            var type = souceDataType;
            if (typeof(IEnumerable<>).IsAssignableFrom(type)
                || typeof(IQueryable<>).IsAssignableFrom(type)
                || typeof(ISet<>).IsAssignableFrom(type)
                )
            {

            }
            else if (type.IsClass)
            {

            }
            return type;
            
        }




    }
}

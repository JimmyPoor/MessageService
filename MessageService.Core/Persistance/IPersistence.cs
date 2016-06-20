using System.Threading.Tasks;

namespace MessageService.Core.Persistance
{
    public interface IPersistence
    {
        Task<StorageResult> Persist<T>(IStorage storage,IStorageSource<T> source);
    }
}

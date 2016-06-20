using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace MessageService.Core.Serializor
{
    public class DefaultSerilizer : IMessageSerilizer
    {
        public Settings Settings { get; set; }

        public bool IsEnable { get; private set; }

        public string DefaultName => "MessageSerilizer";

        public Task<T> Dsearilize<T>(byte[] byteArray)
        {
            if (Util.AssistClass.IsNull(byteArray)) return null;
            var obj = (T)ByteArrayToObject(byteArray);
            return Task.FromResult(obj);
        }

        public Task<byte[]> Searilize<T>(T t)
        {
            if (Util.AssistClass.IsNull(t)) return null;
            var bytes= ObjectToByteArray(t);
            return Task.FromResult(bytes);
        }
        private byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);

            return ms.ToArray();
        }

        private Object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object)binForm.Deserialize(memStream);

            return obj;
        }

        public Task Setup(Settings settings = null)
        {
            IsEnable = true;
            return Task.FromResult(0);
        }

        public void Dispose()
        {
            IsEnable = false;
        }
    }
}

using System;

namespace MessageService.Core.Message
{
    [Serializable()]
    public class ServiceMessage : IServiceMessage
    {
        public byte[] Body { get; protected set; }

        public Settings Headers { get; protected set; }

        public virtual string MessageId => Guid.NewGuid().ToString();

        public virtual int Size => Body.Length;

        public ServiceMessage(byte[] body, Settings headers)
        {
            this.Body = body;
            this.Headers = headers ?? new Settings();
        }

        public static IServiceMessage CreateMessage(object message, Settings settings)
        {
            Util.AssistClass.ExceptionWhenNull(message);

            if (message is IServiceMessage)
                return message as IServiceMessage;
            var serializer = settings.GetSerilizerFromSettings();
            var body = serializer.Searilize(message).Result;
            return new ServiceMessage(body, settings);
        }
    }

    [Serializable]
    public class ServiceReplyMessage : ServiceMessage, IReplyMessage
    {

        public ServiceReplyMessage(byte[] body, Settings headers)
            :base(body,headers)
        {

        }
        public string ReplyTarget { get; private set; }

        public void SetReplyTarget(string target)
        {
            this.ReplyTarget = target;
        }
    }

    [Serializable]
    public class ServiceSubscribeMessage : ServiceMessage, ISubscribeMessage
    {
        public string SubscribeTarget { get; private set; }

        public ServiceSubscribeMessage(string subscribeTarget, byte[] body, Settings headers)
            :base(body,headers)
        {
            this.SubscribeTarget = subscribeTarget;
        }
    }

}

namespace MessageService.Core
{
    public interface IServiceMessage
    {
        string MessageId { get; }
        byte[] Body { get; }
        Settings Headers { get; }
        int Size { get; }
    }


    public interface ISubscribeMessage : IServiceMessage
    {
        string SubscribeTarget { get; }
    }


    public interface IReplyMessage : IServiceMessage
    {
        string ReplyTarget { get;}

        void SetReplyTarget(string target);
    }
}

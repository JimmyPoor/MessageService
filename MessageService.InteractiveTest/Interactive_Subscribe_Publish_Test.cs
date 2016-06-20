using MessageService.Core;
using MessageService.Core.EndPoint;
using MessageService.Core.Message;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.InteractiveTest
{

    public class Interactive_Subscribe_Publish_Test
    {

        string subReceiveEndpointName = "Hello";
        string subRequetsEndpointName = "Hello2";
        Settings settings = new Settings();
        public Interactive_Subscribe_Publish_Test()
        {
            var body = Encoding.UTF8.GetBytes("Hello World");
            var subMessage = new SubscribeMessage(subRequetsEndpointName, body, null);
            var sendSubRequestProcessor = ProcessorCreater.CreateSubRequestProcessor(subRequetsEndpointName, subReceiveEndpointName, subMessage);
            var receiveSubscribeProcessor = ProcessorCreater.CreateSubReceiveProcessor(subReceiveEndpointName);

            sendSubRequestProcessor.Start();
            receiveSubscribeProcessor.Start();

        }
    }

    public class Interactive_Send_Receive_Test2
    {
        string[] subReceiveEndpointName = { "Hello", "Hello4", "Hello5" };
        string[] subRequestEndpointNames = { "Hello1", "Hello2", "Hello3" };
        public Interactive_Send_Receive_Test2()
        {
            //var body = Encoding.UTF8.GetBytes("Hello World");
            //var subMessage = new SubscribeMessage(subRequetsEndpointName, body, null);
            foreach (var name in subReceiveEndpointName)
            {
                var receiveSubscribeProcessor = ProcessorCreater.CreateSubReceiveProcessor(name);
                receiveSubscribeProcessor.Start();
            }
            var i = 0;
            foreach(var name in subRequestEndpointNames)
            {
                var sendSubRequestProcessor = ProcessorCreater.CreateSubRequestProcessor(name, subReceiveEndpointName[i],null);
                sendSubRequestProcessor.Start();
                i++;
            }
        }
    }

    #region 

    public class SubscribeContext : InteractiveContext
    {

    }

    [Serializable]
    public class SubscribeMessage : InteractiveSubscribeMessage
    {
        public SubscribeMessage(string subTarget, byte[] body, Settings headers)
            : base(subTarget, body, headers)
        {

        }
    }

    [Serializable]
    public class PublishMessage : InteractiveMessage
    {
        public PublishMessage(byte[] body)
          : base(body, null)
        {

        }
    }
    public class SubscribeMessageInvoker : IMessageInvoker<SubscribeMessage>
    {
        public Task Invoke(SubscribeMessage msg, MessageInvokeContext ctx)
        {
            //lock (this)
            //{
                //if (ctx.InnerContext.CurrentEndpoint.SubscribeTargets.Count == 3)
                //{
                    for (var i = 0; i < 10000; i++)
                    {
                        var body = Encoding.UTF8.GetBytes("publish messag");
                        var publishMessage = new PublishMessage(body);
                        ctx.Publish(publishMessage);
                    }
               // }
                return Task.FromResult(0);
            //}
        }
    }
    public class PublishMessageInvoker : IMessageInvoker<PublishMessage>
    {
        int i = 0;
        public Task Invoke(PublishMessage msg, MessageInvokeContext ctx)
        {
                Console.WriteLine(i++);
                return Task.FromResult(0);
        }
    }
    #endregion
}

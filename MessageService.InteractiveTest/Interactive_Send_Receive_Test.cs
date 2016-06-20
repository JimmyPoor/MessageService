using MessageService.Core;
using MessageService.Core.EndPoint;
using MessageService.Core.Message;
using MessageService.Core.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageService.InteractiveTest
{
    public class Interactive_Send_Receive_Test
    {
        // issue:
        // 1 未标记为可序列化
        // 2 send without message type
        // 3 step errors test and collect
        // 4 bus start or not
        // 5 forget initial issue


        string endpointName = "Hello";
        string body = "hello world";
        Settings settigns = new Settings();
        public Interactive_Send_Receive_Test()
        {

            new InteractiveProcessor<Context>()
                .BindEndpoint(new ServiceEndpoint(new EndpointDefinition(endpointName, new EndpointPhysicalAddress())))
                .BindSendAction((bus, ctx) =>
                {
                    System.Threading.AutoResetEvent wait = new System.Threading.AutoResetEvent(false);
                    var num = 10000;
                    Thread[] th = new Thread[num];
                    for (int i = 0; i < num; i++)
                    {
                        // Thread.Sleep(1);
                        var bodyBytes = Encoding.UTF8.GetBytes(body + i);
 
                        var thread=new Thread(new ThreadStart(() =>
                        {
                   
                            bus.Send(endpointName, new Message(bodyBytes, null));
                            wait.WaitOne();

                        }));
                        th[i] = thread;
                        th[i].Start();
                        wait.Set();
                    }
                    Thread.Sleep(1000);
                 
                })
                .BindSendErrorHandler((o, e) => { this.DisplayInnerErrors<SendError, StepError>(e); })
                .BindReceiveErrorHandler((o, e) => { this.DisplayInnerErrors<ReceivingError, StepError>(e); })
                .AddReceiver(settigns.GetMessageListener())
                .End((ctx) =>
                {
                    //ctx.CloseTest();
                    //Console.WriteLine(ctx.CurrentEndpoint.EndpointName);
                    //Console.WriteLine(ctx.CurrentMessage);
                })
                .Start();
        }


        private void DisplayInnerErrors<Err, InnerErr>(ErrorEventArgs<Err> e) where Err : Error
        {
            var innerErrors = e.Errors.SelectMany(err => err.InnerErrors as List<InnerErr>);
            foreach (var error in innerErrors)
            {
                Console.WriteLine(error.ToString());
            }
        }
    }

    public class Context : InteractiveContext
    {
    }

    [Serializable]
    public class Message : InteractiveMessage
    {
        public Message(byte[] body, Settings headers) : base(body, headers)
        {
        }
        //public string Content => "Hello world";
    }

    public static class Lock
    {
        public static string LockObj = string.Empty;
        public static string LockObj2 = string.Empty;
        public static int temp = 0;
        public static AutoResetEvent myResetEvent = new AutoResetEvent(false);
    }

    public class ReceiveInvoker : IMessageInvoker<Message>
    {
        int i = 0;
        public Task Invoke(Message msg, MessageInvokeContext ctx)
        {
            lock (this)
            {
                Console.WriteLine("i={0}", i);
               // ctx.Reply(new MyReplyMessage(Encoding.UTF8.GetBytes(i.ToString())));
                i++;

                return Task.FromResult(0);
            }

        }
    }

    [Serializable]
    public class MyReplyMessage : ServiceReplyMessage
    {
        public MyReplyMessage(byte[] body)
            : base(body, null)
        {

        }
    }
    public class ReplyInvoker : IMessageInvoker<MyReplyMessage>
    {
        int j = 0;
        public Task Invoke(MyReplyMessage msg, MessageInvokeContext ctx)
        {
            lock (Lock.LockObj2)
            {
                Console.WriteLine("j={0}", j);
                j++;

                return Task.FromResult(0);
            }
        }
    }

}

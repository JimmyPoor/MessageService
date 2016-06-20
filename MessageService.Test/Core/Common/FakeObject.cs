using MessageService.Core;
using MessageService.Core.Message;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using util = MessageService.Core.Util;

namespace MessageService.Test.Core.Common
{
    [Serializable()]
    public class FakeMessage : ServiceMessage
    {
        public FakeMessage(byte[] body, Settings headers):base(body,headers)
        {
        }
    }

    public class FakeEndpoint : MessageService.Core.EndPoint.ServiceEndpoint
    {
        public FakeEndpoint(string currentName)
            : base(new FakeEndpointDefinition(currentName))
        {
        }

        public class FakeEndpointDefinition : MessageService.Core.EndPoint.EndpointDefinition
        {
            public FakeEndpointDefinition(string currentName)
               : base(util.StaticStringDefinition.DEFAULT_ENDPOINT_NAME, new FakeAddress())
            {
                if (!util.AssistClass.StringAssist.IsNullOrEmpty(currentName))
                    this.BindEndpointName(currentName);
            }
        }

        public class FakeAddress : MessageService.Core.EndPoint.EndpointPhysicalAddress
        {
            public FakeAddress() : base("127.0.0.1", "localhost", 80, null) { }
        }

    }


    public class FakeMiddleware : MessageService.Core.Middleware.ServiceMiddleware
    {
        public FakeMiddleware(FakeMiddlewareBehavior behavior)
           : base(behavior)
        {

        }
    }

    public class FakeMiddlewareBehavior : MessageService.Core.Middleware.IMiddlewareBehavior
    {
        List<Task> _fakeReceiverTasks = new List<Task>();

        public static int simulateReceiveCount = 3;

        public string DefaultName => "MiddlewareBehavior";

        public Settings Settings
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsEnable
        {
            get;
            private set;
        }

        /// <summary>
        /// this simulation for the scenario which listener will receve multi request in same time
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="ctx"></param>
        /// <param name="whenReceive"></param>
        /// <returns></returns>
        public Task Listen(string queue, ProcessContext ctx, Action<Task<byte[]>, ProcessContext, Action<ProcessContext>> whenReceive, Action<ProcessContext> callback)
        {
            if (whenReceive != null)
            {
                for (int i = 0; i < simulateReceiveCount; i++)
                {
                    var content = Task.FromResult(Encoding.UTF8.GetBytes("Hello" + i));
                    var task = Task.Run(() => { whenReceive(content, ctx, callback); });
                    _fakeReceiverTasks.Add(task);
                }
                Task.WaitAll(_fakeReceiverTasks.ToArray());
            }

            return Task.FromResult(0);
        }
        public Task Send(string queue, byte[] message, ProcessContext ctx, Action<ProcessContext> callback)
        {
            return Task.FromResult(0);
        }
        public Task Send(string queue, byte[] message)
        {
            return Task.FromResult(0);
        }

        public Task Close()
        {
            throw new NotImplementedException();
        }

        public Task Setup(Settings settings = null)
        {
            this.IsEnable = true;
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            this.IsEnable = false;
            throw new NotImplementedException();
        }
    }

    public class FakeInvoker : MessageService.Core.Message.IMessageInvoker<Common.FakeMessage>
    {
        public static int InvokeCount = 0;
        public Task Invoke(FakeMessage msg, MessageService.Core.Message.MessageInvokeContext ctx)
        {
            InvokeCount++;
            return Task.FromResult(InvokeCount);
        }
    }

    public class FakeSender : MessageService.Core.Middleware.MiddlewareSender
    {
        public FakeSender()
            : base(
                 new FakeMiddleware(new FakeMiddlewareBehavior())
                 )
        {
        }
    }

    public class FakeListener : MessageService.Core.Middleware.MiddlewareLisenter
    {
        public FakeListener()
            : base(
                 new FakeMiddleware(new FakeMiddlewareBehavior())
                 )
        {
            ReceiveList.Clear();
        }

        private object _lockobj = new object();
        public static List<ProcessContext> ReceiveList = new List<ProcessContext>();
        //protected override void WhenReceive(Task<byte[]> receiveBytesTask, ProcessContext context)
        //{
        //    //var ctx = context.Clone() as ProcessContext;
        //    //var str = "";
        //    //var bytes = receiveBytesTask.Result;
        //    //for (int i = 0; i < bytes.Length; i++)
        //    //{
        //    //    str += bytes[i];
        //    //}
        //    //ctx.CurrentMessage = ServiceMessage.CreateMessage();
        //    //ReceiveList.Add(ctx);
        //}
    }

}

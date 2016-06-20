using MessageService.Core;
using MessageService.Core.Message;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Test.Core.Middleware
{
    [TestFixture]
    public class FakeMiddlewareTest
    {
        Common.FakeMiddleware _middle;
        Common.FakeMiddlewareBehavior _bahavior;
        Settings _settings;
        [SetUp]
        public void Setup()
        {
            _bahavior = new Common.FakeMiddlewareBehavior();
            _middle = new Common.FakeMiddleware(_bahavior);
            _settings = new Settings();
        }

        [Test]
        public void Middleware_Setup_Test()
        {
            _middle.Setup(_settings);
            var sender = _settings.GetMessageOperation();
            var listener = _settings.GetMessageListener();
            var receiveEndpointNames = _middle.ReceveFromEnpointNames;
            var sendEndpointNames =_middle.SendToEndpointNames;

            Assert.IsNotNull(_middle.Behavior);
            Assert.IsTrue(_middle.IsEnable);
            Assert.IsFalse(_middle.IsWorking);
            Assert.IsNotNull(sender);
            Assert.IsNotNull(listener);
            Assert.IsTrue(receiveEndpointNames != null && receiveEndpointNames.Count == 0);
            Assert.IsTrue(sendEndpointNames != null && sendEndpointNames.Count == 0);
        }
    }

    [TestFixture]
    public class MiddlewareSenderAndReceiverTest
    {
        IMessageOperation _sender;
        IMessageLisenter _listener;
        int _sendCount = 3;
        Settings _settings;
        string _queue = "Hello";
        [SetUp]
        public void Setup()
        {
            _sender = new Common.FakeSender();
            _listener = new Common.FakeListener();

            _settings = new Settings();
            _sender.Setup(_settings);
            _listener.Setup(_settings);
            // _sender = _settings.GetMessageOperation();
            //  _listener = _settings.GetMessageListener();
        }
        [Test]
        public void Send_Message_with_full_paras_and_single_thread()
        {
            var body = 10001;
            var message = ServiceMessage.CreateMessage(body, _settings);
            var processCtx = CreateContext();
            _sender.Send(_queue, message, processCtx, (ctx)=> {
                Assert.AreNotSame(processCtx,ctx);// ctx should be copy of processCtx
                Assert.AreSame(ctx.CurrentMessage,message);
            });
        }

        [Test]
        public void Send_Message_with_full_paras_and_mulit_thread()
        {
            List<ProcessContext> resultCtx = new List<ProcessContext>();
            List<Task> tasks = new List<Task>();
            var processCtx = CreateContext();
            for (int i = 0; i < _sendCount; i++)
            {
                var msgStr = "hello" + i;
                var target = _queue;
                processCtx.SendToEndpointName = target;
                var message = new Common.FakeMessage(Encoding.UTF8.GetBytes(msgStr), _settings);
                tasks.Add(
                    Task.Run(() => _sender.Send(target, message, processCtx, (ctx) =>
                    {
                        resultCtx.Add(ctx);
                    }))
                    );
            }
            Task.WaitAll(tasks.ToArray());

            Assert.AreEqual(resultCtx.Count, _sendCount);
        }

        [Test]
        public void Receive_message_test()
        {
            var processCtx = CreateContext();
            processCtx.BindEndpointName(_queue);
            _listener.Listen(processCtx,(ctx)=> {
                Assert.IsNotNull(ctx);
                Assert.AreSame(processCtx,ctx);
            });

        }

        private ProcessContext CreateContext()
        {
            return ProcessContext.Create(_settings);
        }
    }
}
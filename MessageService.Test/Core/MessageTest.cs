using MessageService.Core;
using MessageService.Core.Message;
using MessageService.Test.Core.Common;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MessageService.Test.Core
{
    [TestFixture]
    public class MessageTest
    {

        InvokeContextMapper _mapper;
        Assembly[] _assemblies;
        Assembly _testAssembly;
        Settings _settings;
        [SetUp]
        public void Setup()
        {
            _settings = new Settings();
            _mapper = new InvokeContextMapper();
            _assemblies = AppDomain.CurrentDomain.GetAssemblies();
            _testAssembly = _assemblies.FirstOrDefault(x => x.FullName.Contains("MessageService.Test"));
        }


        [Test]
        public void Binding_Invoker_Test()
        {
            MessageInvokerManager.BindInvokers(_testAssembly);
            var invokersCount = MessageInvokerManager.InvokerCount;
            var invokerType1 = MessageInvokerManager.GetInvokerTypeByMessageType(typeof(FakeMessage));
            var invokerType2 = MessageInvokerManager.GetInvokerTypeByMessageType(typeof(ServiceMessage));
            Assert.GreaterOrEqual(invokersCount, 1);
            Assert.AreEqual(typeof(FakeInvoker), invokerType1);
            Assert.AreEqual(typeof(MessageInovker), invokerType2);
        }


        [Test]
        public void Message_Invoker_Test_with_illegal_null_paras()
        {
            MessageInvokerManager.BindInvokers(_testAssembly);
            Assert.Catch(() =>
            {
                MessageInvokerManager.FireInvokerByMessage(new ServiceMessage(null, null), null, _mapper.Mapping);
            }, "illegal null paras");
        }

        [Test]
        public async void Message_Invoker_Test()
        {
            var message = new ServiceMessage(null, _settings);
            var processContext = CreateContext();
            await MessageInvokerManager.BindInvokers(_testAssembly);
            await MessageInvokerManager.FireInvokerByMessage(message, processContext, _mapper.Mapping);
            Assert.AreSame(MessageInovker.Message, message);
            Assert.AreEqual(MessageInovker.Context.InnerContext, processContext);
        }

        [Test]
        public async void MultiMessage_with_same_invoker_test()
        {
            var processContext = CreateContext();
            MultiDemoMessageInvoker.Clear();
            var message2 = new DemoMessage2();
            var message3 = new DemoMessage3();
            await MessageInvokerManager.BindInvokers(_testAssembly);
            await MessageInvokerManager.FireInvokerByMessage(message2, processContext, _mapper.Mapping);
            await MessageInvokerManager.FireInvokerByMessage(message3, processContext, _mapper.Mapping);
            var invokers = MultiDemoMessageInvoker.invokers;

            Assert.AreEqual(MultiDemoMessageInvoker.requestCount,2);
            Assert.AreEqual(invokers.Count, 2);
            Assert.AreSame(invokers[0],invokers[1]);
        }

        [Test]
        public void Multi_Message_Invoker_Test()
        {
            MessageInvokerManager.BindInvokers(_testAssembly);
            List<IServiceMessage> messages = new List<IServiceMessage>() {
                 new ServiceMessage(null, _settings),
                new DemoMessage()
        };
            List<Task> ls = new List<Task>();
            foreach (var message in messages)
            {
                ls.Add(Task.Run(() =>
                {
                    var context = this.CreateContext();
                    MessageInvokerManager.FireInvokerByMessage(message, context, _mapper.Mapping);
                }));
            }
            Task.WaitAll(ls.ToArray());

        }

        [Test]
        public void Mapping_between_processContext_to_invokeContext_Test()
        {
            var mapping = _mapper.Mapping;
            Assert.IsNotNull(mapping);
            var context = this.CreateContext();
            var invokerContext = mapping(context);
            Assert.IsNotNull(invokerContext);
        }

        private ProcessContext CreateContext()
        {
            return ProcessContext.Create(_settings);
        }


        public class DemoMessage : ServiceMessage
        {
            public DemoMessage()
               : base(null, new Settings())
            {

            }
        }

        public class DemoMessage2 : ServiceMessage
        {
            public DemoMessage2()
                : base(null, new Settings())
            {

            }
        }

        public class DemoMessage3 : ServiceMessage
        {
            public DemoMessage3()
                : base(null, new Settings())
            {

            }
        }

        public class DemoMessageInvoker : IMessageInvoker<DemoMessage>
        {
            public Task Invoke(DemoMessage msg, MessageInvokeContext ctx)
            {
                return Task.FromResult(0);
            }
        }
        public class MultiDemoMessageInvoker : IMessageInvoker<DemoMessage2>,
                                                                           IMessageInvoker<DemoMessage3>
        {
            public static int requestCount = 0;

            public static List<MultiDemoMessageInvoker>  invokers = new List<MultiDemoMessageInvoker>();

            public static void Clear()
            {
                requestCount = 0;
                invokers.Clear();
            }

            public Task Invoke(DemoMessage2 msg, MessageInvokeContext ctx)
            {
                requestCount++;
                invokers.Add(this);
                return Task.FromResult(0);
            }

            public Task Invoke(DemoMessage3 msg, MessageInvokeContext ctx)
            {
                requestCount++;
                invokers.Add(this);
                return Task.FromResult(0);
            }
        }

        public class MessageInovker :
             IMessageInvoker<ServiceMessage>
        {
            public static ServiceMessage Message { get; private set; }

            public static MessageInvokeContext Context { get; private set; }
            public Task Invoke(ServiceMessage msg, MessageInvokeContext ctx)
            {
                Message = msg;
                Context = ctx;
                return Task.FromResult(0);
            }
        }



    }
}

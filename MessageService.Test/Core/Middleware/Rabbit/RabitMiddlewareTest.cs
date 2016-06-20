using MessageService.Core;
using MessageService.Core.Container.Unity;
using MessageService.Core.Middleware.RabbiMQ;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Test.Core.Middleware.Rabbit
{
    [TestFixture]
    public class RabitMiddlewareTest
    {
        [SetUp]
        public void Setup()
        {

        }
    }

    [TestFixture]
    public class RabbitMiddlewareBehaviorTest
    {
        private RabbitMQMiddlewareBehavior _behavior;
        private RabbitMQ.Client.IModel _channel;
        private RabbitMQ.Client.IConnection _connection;
        private string _queue => "Hello";
        private int _sendCount = 3;
        [SetUp]
        public void Setup()
        {
            _behavior = new RabbitMQMiddlewareBehavior();
        }

        [Test]
        public void Behavior_Send_Message_With_Single_Thread_Test()
        {
            if (_channel != null && _channel.IsOpen)
            {
                _channel.Close();
            }
            var message = Encoding.UTF8.GetBytes("Hello Rabbit2");
            _behavior.Send(_queue, message);
            _channel = _behavior.Context.SendChannel;
            _connection = _behavior.Context.SendConnection;
            Assert.IsNotNull(_channel);
            Assert.IsTrue(_channel.IsClosed);
            Assert.IsFalse(_connection.IsOpen);
        }

        [Test]
        public void Behavior_Send_Message_With_Multi_Thread_Test()
        {
            List<Task> taskList = new List<Task>();
            for (int i = 0; i < this._sendCount; i++)
            {
                var message = Encoding.UTF8.GetBytes("message "+i);
                taskList.Add(Task.Run(()=> {
                    var processCtx = CreateContext();
                    _behavior.Send(_queue, message,processCtx,(ctx)=> {
                        var msg = Encoding.UTF8.GetString(ctx.CurrentMessage.Body as byte[]);
                        Assert.IsNotNull(msg);
                    });
                }));
            }

            Task.WaitAll(taskList.ToArray());
            Assert.AreEqual(taskList.Count, _sendCount);

        }

        [Test]
        public void Behavior_Received_Once_Test()
        {
            ReceiveMessage(true);
        }

        [Test]
        public void Behavior_Received_Test()
        {
            ReceiveMessage(false);
        }

        [Test]
        public void Behavoir_Close_Test()
        {

        }

        private ProcessContext CreateContext()
        {
            return new ProcessContext(new UnityContainer(), new Settings());
        }

        private void ReceiveMessage(bool stop)
        {
            _behavior.Listen(_queue, CreateContext(), (t, ctx,callback) =>
            {
                var context = ctx.Clone();
                var consumerCount = _behavior.Context.ReceiveChannel.ConsumerCount(_queue);
                var msg = Encoding.UTF8.GetString(ctx.CurrentMessage.Body as byte[]);

                Assert.IsNotNull(msg);
                Assert.IsNotNull(context);
                Assert.IsNotNull(t);
                Assert.IsNotNull(t.Result);
                Assert.GreaterOrEqual(consumerCount, 1);
                if (stop)
                {
                    _behavior.Close();
                    Assert.IsTrue(_behavior.Context.ReceiveChannel.IsClosed);
                    Assert.IsTrue(!_behavior.Context.ReceiveConnection.IsOpen);
                }
              
            });
        }



    }

}

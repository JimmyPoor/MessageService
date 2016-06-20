using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading.Tasks;

namespace MessageService.Core.Middleware.RabbiMQ
{
    public class RabbitMQMiddlewareBehavior : IMiddlewareBehavior
    {

        public RabbitMQMiddlewareBehavior()
        {
            this.BuildContext();
        }
        public RabbitMQContext Context { get; private set; }

        public string DefaultName => "MiddlewareBehavior";

        public Settings Settings { get; set; }

        public bool IsEnable { get; private set; }

        public Task Listen(
            string queue, 
            ProcessContext ctx, 
            Action<Task<byte[]>, ProcessContext, Action<ProcessContext>> whenReceive, 
            Action<ProcessContext> callback = null)
        {
            Context.ReceiveConnection = RabbitMQContext.CreateConnection();
            var conn = Context.ReceiveConnection;
            Context.ReceiveChannel = RabbitMQContext.CreateChannel(conn);
            conn.ConnectionShutdown += WhenReceiveConnShutdown;
            conn.ConnectionBlocked += Conn_ConnectionBlocked;
            var channel = Context.ReceiveChannel;
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (o, e) =>
            {
                if (!Util.AssistClass.IsNull(whenReceive))
                {
                    whenReceive(Task.FromResult(e.Body), ctx,callback);
                }
            };
            try
            {
                channel.BasicQos(0, 1, false);
                channel.BasicConsume(queue, true, consumer);
            }
            catch (Exception) {// will add log later

            }
            return Task.FromResult(true);
        }

        protected virtual void Conn_ConnectionBlocked(object sender, RabbitMQ.Client.Events.ConnectionBlockedEventArgs e)
        {
        }

        protected virtual void WhenReceiveConnShutdown(object o, ShutdownEventArgs args)
        {
        }

        public Task Send(string queue, byte[] message)
        {
           // System.Threading.Thread.Sleep(1);
                return this.Send(queue, message, null, null);
        }

        public Task Send(string queue, byte[] body, ProcessContext ctx, Action<ProcessContext> callback)
        {
            Util.AssistClass.ExceptionWhenNull(queue);
            Util.AssistClass.ExceptionWhenNull(body);
            Context.SendConnection = RabbitMQContext.CreateConnection();
            using (var conn = Context.SendConnection)
            {
                Context.SendChannel = RabbitMQContext.CreateChannel(conn);
                using (var channel = Context.SendChannel)
                {
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2;
                    Context.SendChannel.BasicPublish(string.Empty, queue, properties, body);
                    if (!Util.AssistClass.IsNull(callback) && !Util.AssistClass.IsNull(ctx))
                    {
                        callback(ctx);
                    }
                }
            }
            return Task.FromResult(true);
        }

        private void BuildContext()
        {
            this.Context = RabbitMQContext.Build();
        }

        public Task Close()
        {
            if (!Util.AssistClass.IsNull(Context))
            {
                if (Context.ReceiveChannel.IsOpen)
                    Context.ReceiveChannel.Close();
                if (Context.ReceiveConnection.IsOpen)
                    Context.ReceiveConnection.Close();
            }
            return Task.FromResult(0);
        }

        public Task Setup(Settings settings = null)
        {
            this.IsEnable = true;
            return Task.FromResult(0);
        }

        public void Dispose()
        {
          this.IsEnable = false;
        }
    }
}

using RabbitMQ.Client;

namespace MessageService.Core.Middleware.RabbiMQ
{
    public class RabbitMQContext
    {
        public IConnection SendConnection { get; internal set; }
        public IConnection ReceiveConnection { get; internal set; }
        public IModel SendChannel { get; set; }
        public IModel ReceiveChannel { get; set; }

        public static RabbitMQContext Build()
        {
            return new RabbitMQContext();
        }

        public static IConnection CreateConnection(string host = null, string userName = null, string password = null)
        {
            var factory = new ConnectionFactory();
            factory.HostName = host ?? "127.0.0.1";
            factory.UserName = userName ?? "kissnana3";
            factory.Password = password ?? "kisskiss";
            return factory.CreateConnection();
        }

        public static IModel CreateChannel(IConnection conn)
        {
            return conn.CreateModel();
        }

    }
}

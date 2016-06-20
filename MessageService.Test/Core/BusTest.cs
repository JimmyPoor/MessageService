using MessageService.Core;
using MessageService.Core.Bus;
using MessageService.Core.Container.Unity;
using MessageService.Core.EndPoint;
using MessageService.Core.Message;
using MessageService.Core.Middleware;
using MessageService.Core.Middleware.RabbiMQ;
using MessageService.Core.Persistance;
using MessageService.Core.Serializor;
using NUnit.Framework;
using System.Linq;

namespace MessageService.Test
{
    [TestFixture]
    public class ServiceBusTest
    {
        ServiceBus _bus;
        Settings _settings;
        string _endpointName = "Hello";
        [SetUp]
        public void Setup()
        {
            _bus = new ServiceBus();
            _settings = new Settings();
        }

        [Test]
        public void Bus_initial_test()
        {
            _bus.Initial(_settings);

            Assert.IsInstanceOf(typeof(IObjectContainer), _settings["Container"]);
            Assert.IsInstanceOf(typeof(ServiceEndpoint), _settings["Endpoint"]);
            Assert.IsInstanceOf(typeof(EndpointDefinition), _settings["EndpointDefinition"]);
            Assert.IsInstanceOf(typeof(IMiddlewareBehavior), _settings["MiddlewareBehavior"]);
            Assert.IsInstanceOf(typeof(IMiddleware), _settings["Middleware"]);
            Assert.IsInstanceOf(typeof(ServiceEndpoint), _settings["Endpoint"]);

            //IOC result test

            var container = _settings.GetContainer();
            var middlewareBehavior = container.Resolve<IMiddlewareBehavior>();
            var middleware = container.Resolve<IMiddleware>();
            var serializor = container.Resolve<IMessageSerilizer>();
            var bahavior = container.Resolve<IMiddlewareBehavior>();

            Assert.IsNotNull(container);
            Assert.IsNull(middlewareBehavior);
            Assert.IsNull(middleware);
            Assert.IsNull(serializor);

            // _bus.Start();
            // _bus.Stop();
            // Assert.IsNotNull(persistence);
        }

        [Test]
        public void Bus_intitial_with_components()
        {
            _bus.BindMessageListener(new MiddlewareLisenter())
                  .BindMessageOperation(new MiddlewareSender())
                  .BindMessageSerilizer(new DefaultSerilizer())
                  .UseMiddleware(new ServiceMiddleware(new RabbitMQMiddlewareBehavior()))
                  .Initial(_settings);

            Assert.AreEqual(_bus.Components.Count, 4);

            var container = _settings.GetContainer();
            var middleware = container.Resolve<IMiddleware>();
            var serializor = container.Resolve<IMessageSerilizer>();
            var sender = container.Resolve<IMessageOperation>();
            var listener = container.Resolve<IMessageLisenter>();

            Assert.IsNotNull(container);
            Assert.IsNotNull(middleware);
            Assert.IsNotNull(serializor);
            Assert.IsNotNull(listener);
            Assert.IsNotNull(sender);
        }

        [Test]
        public async void Bus_Start_Stop_with_Components()
        {
            _bus.BindMessageListener(new MiddlewareLisenter())
                .BindMessageOperation(new MiddlewareSender())
                .BindMessageSerilizer(new DefaultSerilizer())
                .UseMiddleware(new ServiceMiddleware(new RabbitMQMiddlewareBehavior()))
                .Initial(_settings);

            await _bus.Start();

            Assert.IsTrue(
                _bus.Components.All(com => com.IsEnable == true)
                );

            await _bus.Stop();
            _bus.Dispose();
            Assert.IsTrue(
                  _bus.Components.All(com => com.IsEnable == false)
                );


        }

        [Test]
        public void Bus_bind_null_throw_test()
        {
            Assert.Catch(() =>
            {
                _bus.WithEndpoint(null);
                //_bus.WithEndpointName(null);
            });
            Assert.Catch(() =>
            {
                _bus.BindContainer(null);
            });
        }

        [Test]
        public void Bus_bind_test()
        {
            var container = new UnityContainer();
            var serializer = new DefaultSerilizer();
            _bus.BindContainer(container)
                   .BindMessageSerilizer(serializer);
            //  .WithEndpointName(_endpointName);
            _bus.Initial(_settings);
            var endpoint = _settings.GetEndpoint();
            Assert.AreSame(container, _settings["Container"]);
            Assert.AreSame(serializer, _settings["MessageSerilizer"]);
            Assert.AreEqual(endpoint.EndpointName, _endpointName);
        }
    }
}

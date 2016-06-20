using MessageService.Core;
using MessageService.Core.EndPoint;
using MessageService.Test.Core.Common;
using NUnit.Framework;
using System.Text;

namespace MessageService.Test.Core
{
    [TestFixture]
    public class EndpointTest
    {
        FakeEndpoint _endpoint;
        string _sendMessage = "hello";
        string _target = "Hello";
        Settings _settings = new Settings();
        [SetUp]
        public void Setup()
        {
            _endpoint = new FakeEndpoint(_target); //binding endpoint name as target and listen target
            _endpoint.Setup(_settings);
        }

        [Test]
        public void SetupTest()
        {
            Assert.IsNotNull(_endpoint.Settings);
            Assert.IsNotNull(_endpoint.Identity);
            Assert.IsNotNull(_endpoint.EndpointName);
            Assert.IsNotNull(_endpoint.Definition);
            Assert.IsTrue(_endpoint.IsEnable);
            Assert.IsFalse(_endpoint.IsWorking);
        }

        [Test]
        public void Endpoint_after_starting_test()
        {
            _endpoint.Start();
            Assert.IsTrue(_endpoint.IsWorking);
        }

        [Test]
        public void Endpoint_after_stoping_test()
        {
            _endpoint.Stop();
            _endpoint.Dispose();

            Assert.IsFalse(_endpoint.IsWorking);
            Assert.IsFalse(_endpoint.IsEnable);
        }

        [Test]
        public void Endpoint_send_IServiceMessage_test()
        {
            var body = Encoding.UTF8.GetBytes(_sendMessage);
            var fakeMessage = new FakeMessage(body, null);
            _endpoint.Start();
            _endpoint.SendServiceMessage(_target, ()=> fakeMessage, (ctx) =>
            {
                Assert.IsNotNull(ctx);
                Assert.IsNotNull(ctx.CurrentMessage);
                Assert.IsNotNull(ctx.CurrentEndpoint);
                Assert.IsNotNull(ctx.Container);
                Assert.IsNotNull(ctx.Settings);
                Assert.IsNotNull(ctx.SendToEndpointName);
                Assert.AreSame(ctx.CurrentEndpoint, _endpoint);
                Assert.IsTrue(ctx.CurrentMessage is IServiceMessage);

            });
        }


        [Test]
        public void Endpoint_send_non_IServiceMessage_test()
        {
            _endpoint.Start();
            _endpoint.Send(_target, _sendMessage, (ctx) =>
            {
                Assert.IsNotNull(ctx);
                Assert.IsNotNull(ctx.CurrentMessage);
                Assert.IsNotNull(ctx.CurrentEndpoint);
                Assert.IsNotNull(ctx.Container);
                Assert.IsNotNull(ctx.Settings);
                Assert.IsNotNull(ctx.SendToEndpointName);
                Assert.AreSame(ctx.CurrentEndpoint, _endpoint);
                Assert.AreEqual(_sendMessage, ctx.CurrentMessage);
                Assert.IsTrue(ctx.CurrentMessage is IServiceMessage);
            });
        }

        /// <summary>
        /// will use scenario test for listen logic later
        /// </summary>
        [Test]
        public void Endpoint_listen_test()
        {
            _endpoint.Start();
            _endpoint.Listen();
            Assert.IsTrue(FakeListener.ReceiveList.Count > 0);
        }
    }

    [TestFixture]
    public class EndpointContractTest
    {
        FakeEndpoint _endpoint;
        EndpointContract _contract;
        Settings _settings = new Settings();
         
        [SetUp]
        public void Setup()
        {
            _endpoint = new FakeEndpoint(null); //binding endpoint name as target and listen target
            _contract = new EndpointContract();
            _contract.AddEndpointIdentityContract(_endpoint.Identity);
            _contract.AddEndpointNameContract(_endpoint.EndpointName);
        }

        [Test]
        public void Bind_contract_after_setup_throw_error()
        {
            Assert.Catch(
                   () =>
                   {
                       _endpoint.Setup(_settings);
                       _endpoint.BindContract(_contract);
                   },"need bind contract before endpoint set up"
                );
        }

        [Test]
        public void Bind_contract_test()
        {
            _endpoint.BindContract(_contract);
            Assert.AreSame(_endpoint.Contract, _contract);
        }

        [Test]
        public  void Validate_contract_test()
        {
            var result= EndpointDefinition.StartCheckContarct(_contract);
            Assert.IsTrue(result);
        }
    }

    public class EndpointDefinitionTest
    {
   
    }
}

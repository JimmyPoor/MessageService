using MessageService.Core.Message;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Routing.Test
{
    [TestFixture]
    public class RouteTabel_Test
    {
        RouteTable _table;
        IRouteStrategy _defaultRouteStrategy;
        [SetUp]
        public void Setup()
        {
            _table = new RouteTable();
            _defaultRouteStrategy = new TestRouteStrategy("default");
        }

        [Test]
        public void Single_route_map_test()
        {
            _table.RouteMap(typeof(object), _defaultRouteStrategy);
            Assert.AreEqual(_table.RoutesCount,1);
        }

        [Test]
        public void Remove_route_map_test()
        {
            _table.RouteMap(typeof(object), _defaultRouteStrategy);
            _table.RemoveRouteMap(typeof(object));
            Assert.AreEqual(_table.RoutesCount, 0);
        }

        [Test]
        public void Multi_route_map_message_type_test()
        {
            Type[] typeArray = {typeof(Message1),typeof(Message2) };
            foreach (var type in typeArray)
            {
                _table.RouteMap(type, _defaultRouteStrategy);
            }

            Assert.AreEqual(typeArray.Length,_table.RoutesCount);
            Assert.IsTrue(_table.Routes.Keys.Any(x=>x.Equals(typeof(Message2))));
        }



    }

    public class TestRouteStrategy : DefaultRouteStrategy
    {
         public TestRouteStrategy(string name) : base(name)
        {

        }
    }

    public class Message1 { }
    public class Message2 { }
}

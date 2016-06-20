using MessageService.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Routing.Test
{
    [TestFixture]
    public class Router_Test
    {
        Router _router;
        RouteTable _table;
        Settings _settings=new Settings();
        IRouteStrategy _strategy;
        [SetUp]
        public void Setup()
        {
            _table = new RouteTable();
            _router = new Router(_table);
            _strategy = new DefaultRouteStrategy("default");
        }

        [Test]
        public void Router_Setup_Test()
        {
            _router.Setup(_settings);
            Assert.IsNotNull(_settings);
            Assert.AreSame(_router.Table,_table);
            Assert.AreSame(_settings.GetRouteTable(),_table);
            Assert.AreSame(_settings.GetRouter(), _router);
        }

        [Test]
        public void Route_Setup_when_table_null_Test()
        {
            _router = new Router(null);
            _router.Setup(_settings);
            Assert.AreNotSame(_router.Table, _table);
            Assert.AreNotSame(_settings.GetRouteTable(), _table);
            Assert.AreSame(_settings.GetRouter(), _router);
        }

        [Test]
        public void Route_Single_Mapping_Test()
        {
            _router.RouteMap( typeof(RouteMessage), _strategy);
            Assert.AreEqual(_router.Table.RoutesCount, 1);
        }

        [Test]
        public void Route_Multi_Mapping_with_same_message_type_Test()
        {
            var routeCount = 3;
            for (int i = 0; i < routeCount; i++)
            {
                var routeStrategy = new DefaultRouteStrategy("default" + i);
                _router.RouteMap(typeof(RouteMessage), routeStrategy);
            }

            Assert.AreEqual(_router.Table.RoutesCount, 1);
        }

        [Test]
        public void Route_Multi_Mapping_with_diff_message_type_Test()
        {
            var routeStrategy = new DefaultRouteStrategy("default" );
            _router.RouteMap(typeof(RouteMessage), routeStrategy);
            _router.RouteMap(typeof(RouteMessage2), routeStrategy);

            Assert.AreEqual(_router.Table.RoutesCount, 2);
        }

        [Test]
        public void Route_Disable_Test()
        {
         
        }
    }

    public class RouteMessage
    {
    }

    public class RouteMessage2
    {
    }
}

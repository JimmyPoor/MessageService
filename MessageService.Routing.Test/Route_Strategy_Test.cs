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
    public class Route_Strategy_Test
    {
        IRouteStrategy _strategy;
        RouteContext _cxt;
        ProcessContext _pctx = ProcessContext.Create(new Settings());
        [SetUp]
        public void Setup()
        {
            _strategy = new DefaultRouteStrategy("Test_Strategy");
            _cxt = new RouteContext(_pctx);
        }

        [Test]
        public void Add_inner_Strategy_with_Null_options_Test()
        {
            Assert.Catch(() => _strategy.AddInnerStrategy(null, null),
                "add strategy parameter should not be null"
                );
        }

        [Test]
        public void Add_inner_Strategy_Test()
        {
         
            _strategy.AddInnerStrategy((o) => true, (ctx) => null);
            Assert.AreEqual(_strategy.InnerStrategyCount, 1);
        }

        [Test]
        public void Add_Muilt_inner_Strategies_Test()
        {
            var resultCount = 3;
            for (int i = 0; i < resultCount; i++)
            {
                _strategy.AddInnerStrategy((o) => true, (ctx) => null);
            }
            Assert.AreEqual(_strategy.InnerStrategyCount,3);
        }

        [Test]
        public void Invoke_Inner_Strategy_with_true_condition_Test()
        {
            _strategy.AddInnerStrategy((o) => true, (ctx) => {
                var result = new RouteResult();
                result.AddTarget(new EndpointTarget("localhosts"));
                return result;
            });
            var results=_strategy.Invoke("ok", _cxt).Result;
            var targets = results.ToList();
            var target = targets.First();
            Assert.AreEqual(targets.Count(),1); 
            Assert.IsNotNull(target);
            Assert.IsTrue(target.Targets.Any(x=>x.ToString()== "localhosts"));
        }

        [Test]
        public void Inovke_Inner_Strategy_with_true_condition_routeError_test()
        {
            _strategy.AddInnerStrategy((o) => true, (ctx) => {
                var result = new RouteResult();
                result.AddError(new RouteError(null,"Test error"));
                return result;
            });
            var results = _strategy.Invoke("ok", _cxt).Result;
            var targets = results.ToList();
            var target = targets.First();
            Assert.AreEqual(targets.Count(), 1);
            Assert.IsNotNull(target);
            Assert.IsTrue(target.RouteErrors.Any(x => x.ToString() == "Test error"));
        }

        [Test]
        public void Invoke_Inner_Strategy_with_false_conidition_test()
        {
            _strategy.AddInnerStrategy((o)=>false,(ctx)=>null);
            var results = _strategy.Invoke("not good",_cxt).Result;
            Assert.IsNull(results);
        }

        public void Disable_Strategy_Test()
        {
            _strategy.Disable();
            Assert.IsFalse(_strategy.IsEnable);
        }

    }
}

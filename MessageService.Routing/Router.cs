using MessageService.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageService.Routing
{
    public class Router : IComponent
    {
        RouteTable _routeTable;

        public RouteTable Table => _routeTable;

        public string DefaultName => "Router";

        public Settings Settings { get; set; }

        public bool IsEnable { get; private set; }

        public Router(RouteTable table)
        {
            this._routeTable = table;
        }

        public Task<IEnumerable<RouteResult>> Route<Message>(Message message, RouteContext context)
        {
            Core.Util.AssistClass.ExceptionWhenNull(message);
            Core.Util.AssistClass.ExceptionWhenNull(context);
            bool isContains = _routeTable.Routes.Keys.Contains(message.GetType());
            if (isContains)
            {
                var strategy = _routeTable.Routes[message.GetType()];
                if (strategy.IsEnable)
                    return strategy.Invoke(message, context);
            }
            return null;
        }

        public Router RouteMap(Type messageType, IRouteStrategy strategy)
        {
            Core.Util.AssistClass.ExceptionWhenNull(messageType);
            Core.Util.AssistClass.ExceptionWhenNull(strategy);
            _routeTable.RouteMap(messageType, strategy);
            return this;
        }

        public void DisableStrategy(string strategyName)
        {
            foreach(var strategy in Table.Routes.Values)
            {
                if(strategy.StrategyName== strategyName)
                {
                    strategy.Disable();
                }
            }
        }

        public void DisableStrategiesByType(Type messageType)
        {
            var routes = _routeTable.Routes.Where(x => x.Key == messageType);
            if (routes != null)
            {
                foreach(var route in routes)
                {
                    route.Value.Disable();
                }
            }
        }

        public void EnableStrategy(string strategyName)
        {
            foreach (var strategy in Table.Routes.Values)
            {
                if (strategy.StrategyName == strategyName)
                {
                    strategy.Enable();
                }
            }
        }

        public void EnableStrategiesByType(Type messageType)
        {
            var routes = _routeTable.Routes.Where(x => x.Key == messageType);
            if (routes != null)
            {
                foreach (var route in routes)
                {
                    route.Value.Enable();
                }
            }
        }

        public Task Setup(Settings settings = null)
        {
            if (!IsEnable)
            {
                settings = settings ?? new Settings();
                if (Core.Util.AssistClass.IsNull(_routeTable))
                {
                    _routeTable = settings.GetRouteTable();
                }
                settings.Set("RouteTable", _routeTable, true);
                settings.Set("Router", this, true);
                this.Settings = settings;
                this.IsEnable = true;
            }
            return Task.FromResult(0);
        }

        public void Dispose()
        {
            this.IsEnable = false;
        }

    }
}

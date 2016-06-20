using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace MessageService.Routing
{
    public class RouteTable
    {
        public ConcurrentDictionary<Type, IRouteStrategy> Routes => _routes;

        private ConcurrentDictionary<Type, IRouteStrategy> _routes = new ConcurrentDictionary<Type, IRouteStrategy>();

        public int RoutesCount => _routes.Count;

        public Task RouteMap(Type type, IRouteStrategy routeStrategy)
        {
            Core.Util.AssistClass.ExceptionWhenNull(type);
            Core.Util.AssistClass.ExceptionWhenNull(routeStrategy);
            _routes.AddOrUpdate(type, routeStrategy, (t, strategy) => strategy);
            return Task.FromResult(0);
        }

        public Task<bool> RemoveRouteMap(Type type)
        {
            IRouteStrategy _strategy = null;
            var removeResult = _routes.TryRemove(type, out _strategy);
            return Task.FromResult(removeResult);
        }
    }


}

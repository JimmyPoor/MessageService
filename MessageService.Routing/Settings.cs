using MessageService.Core;

namespace MessageService.Routing
{
    public static class SettingExtensions
    {
        public static Router GetRouter(this Settings settings)
        {
            var table = settings.GetRouteTable();
            var router = settings["Router"] as Router ?? new Router(table);
            settings.Set("Router", router);
            return router;
        }

        public static RouteTable GetRouteTable(this Settings settings)
        {
            var routeTable = settings["RouteTable"] as RouteTable ?? new RouteTable();
            settings.Set("RouteTable", routeTable);
            return routeTable;
        }
    }
}

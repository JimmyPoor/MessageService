using MessageService.Core;
using MessageService.Core.EndPoint;
using MessageService.Core.Message;
using MessageService.Core.Middleware;
using MessageService.Core.Steps;
using MessageService.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.InteractiveTest
{

    public class Interactive_Routing_Same_endpoint_Test
    {
        string endpointName = "Hello";
        string body = "hello world";
        Settings settigns = new Settings();

        public Interactive_Routing_Same_endpoint_Test()
        {
            var router = settigns.GetRouter();
            var routeStrategy = CreateFakeRouteStrategy();
            router.RouteMap(typeof(RoutingMessage), routeStrategy);
            var listener = settigns.GetMessageListener() as MiddlewareLisenter;
            listener.AddIncomingStep(typeof(RouteStep).Name);

            new InteractiveProcessor<RoutingContext>()
                .BindEndpoint(new ServiceEndpoint(new EndpointDefinition(endpointName, new EndpointPhysicalAddress())))
                .AddCompontents(router)
                .BindSendAction((bus, ctx) => {
                    var bodyBytes = Encoding.UTF8.GetBytes(body);
                    bus.Send(endpointName, new RoutingMessage(bodyBytes, null));
                })
                .AddReceiver(listener)
                .End((ctx) =>
                {
                    //ctx.CloseTest();
                    //Console.WriteLine(ctx.CurrentEndpoint.EndpointName);
                    //Console.WriteLine(ctx.CurrentMessage);
                })
                .Start();
        }

        private IRouteStrategy CreateFakeRouteStrategy()
        {
            var strategy = new DefaultRouteStrategy("test");
            strategy.AddInnerStrategy(
                o=> 
                {
                    if((o as RoutingMessage) != null)
                    {
                        return true;
                    }
                    return false;
                },
                (ctx) =>
                {
                    var result= new RouteResult();
                    var target = new EndpointTarget(endpointName);
                    result.AddTarget(target);
                    return result;
                }
                );
            return strategy;
        }

    }


    public class Interactive_Routing_Diff_Endpoint_Test
    {
        string _sendEndpoint="Hello";
        string _routerEndpoint="Hello2";
        string _routeTargetEndpoint="Hello3";
        Settings settigns = new Settings();
        string _body = "Hello World";
        public Interactive_Routing_Diff_Endpoint_Test()
        {
            var bodyBytes = Encoding.UTF8.GetBytes(_body);
            var routeMessage= new RoutingMessage(bodyBytes, null);
            var sendRouteRequestProcessor = ProcessorCreater.CreateSubRequestProcessor(_sendEndpoint, _routerEndpoint, routeMessage);
            var routeProcessor = ProcessorCreater.CreateSubReceiveProcessor(_routerEndpoint);
            var routeTargetProcessor= ProcessorCreater.CreateSubReceiveProcessor(_routeTargetEndpoint);


            var router = settigns.GetRouter();
            var routeStrategy = CreateFakeRouteStrategy();
            router.RouteMap(typeof(RoutingMessage), routeStrategy);
            var listener = settigns.GetMessageListener() as MiddlewareLisenter;
            listener.AddIncomingStep(typeof(RouteStep).Name);
            routeProcessor
                .AddCompontents(router)
                .AddReceiver(listener);


            routeTargetProcessor.Start();
            routeProcessor.Start();
            sendRouteRequestProcessor.Start();
        }


        private IRouteStrategy CreateFakeRouteStrategy()
        {
            var strategy = new DefaultRouteStrategy("test");
            strategy.AddInnerStrategy(
                o =>
                {
                    if ((o as RoutingMessage) != null)
                    {
                        return true;
                    }
                    return false;
                },
                (ctx) =>
                {
                    var result = new RouteResult();
                    var target = new EndpointTarget(_routeTargetEndpoint);
                    result.AddTarget(target);
                    return result;
                }
                );
            return strategy;
        }

    }


    [Serializable]
     public class RoutingMessage : InteractiveMessage
    {
          public RoutingMessage(byte[] body,Settings settings) 
            : base(body, settings)
        {

        }
    }

    [Serializable]
    public class RedirectMessage : InteractiveMessage
    {
        public RedirectMessage(byte[] body, Settings settings) 
            : base(body, settings)
        {

        }
    }


    public class RoutingContext : InteractiveContext
    {

    }

    public class RouteInvoker : IMessageInvoker<RoutingMessage>
    {
        public Task Invoke(RoutingMessage msg, MessageInvokeContext ctx)
        {
            lock (this)
            {
                var routeResults = ctx.InnerContext.Settings["RouteResult"] as IEnumerable<RouteResult>;
                if (routeResults != null)
                {
                    var routeResult = routeResults.FirstOrDefault();
                    if (routeResult != null && routeResult.HasRoute)
                    {
                        var message = new RedirectMessage(msg.Body, msg.Headers);
                        var target = routeResult.Targets.First();
                        ctx.Send(target.ToString(), message);
                    }
                }
                return Task.FromResult(0);
            }
        }
    }


    public class RouteTargetInvoker : IMessageInvoker<RedirectMessage>
    {
        public Task Invoke(RedirectMessage msg, MessageInvokeContext ctx)
        {
            throw new NotImplementedException();
        }
    }




}

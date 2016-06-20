using MessageService.Core;
using MessageService.Core.EndPoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.InteractiveTest
{
    public static class ProcessorCreater
    {
        public static InteractiveProcessor<SubscribeContext> CreateSubRequestProcessor(string subRequestEndpointName, string subReceiveEndpointName,IServiceMessage message )
        {
            var subRequestEndpoint = new ServiceEndpoint(new EndpointDefinition(subRequestEndpointName, new EndpointPhysicalAddress()));
            var sendSubRequestProcessor = new InteractiveProcessor<SubscribeContext>()
               .BindEndpoint(subRequestEndpoint)
               .BindSendAction((bus, ctx) =>
               {
                   var body = Encoding.UTF8.GetBytes("subscribe");
                   var subMessage = message;
                   bus.Send(subReceiveEndpointName, subMessage);
               })
               .End((subCtx) => { }
               );
            return sendSubRequestProcessor;
        }

        public static InteractiveProcessor<SubscribeContext> CreateSubReceiveProcessor(string subReceiveEndpointName)
        {
            var subReceiveEndpoint = new ServiceEndpoint(new EndpointDefinition(subReceiveEndpointName, new EndpointPhysicalAddress()));
            var receiveSubscribeProcessor = new InteractiveProcessor<SubscribeContext>()
                 .BindEndpoint(subReceiveEndpoint)
                 .End((subCtx) => { });
            return receiveSubscribeProcessor;
        }
    }
}

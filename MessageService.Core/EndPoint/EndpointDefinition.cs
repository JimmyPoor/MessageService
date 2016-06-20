using MessageService.Core.Util;
using System;
using System.Linq;

namespace MessageService.Core.EndPoint
{
    public class EndpointDefinition
    {
        public EndpointPhysicalAddress EndpointAddress { get; protected set; }
        public string Identity => CreateIdentity();

        public string EndpointName { get; private set; }

        public static string DefultName => StaticStringDefinition.DEFAULT_ENDPOINT_NAME;

        public EndpointDefinition()
        {
            this.EndpointAddress = EndpointDefinition.DefaultAddress;
            this.EndpointName = EndpointDefinition.DefultName;
        }

        public EndpointDefinition(string endpointName, EndpointPhysicalAddress address)
        {
            Util.AssistClass.ExceptionWhenNull(address);
            this.EndpointAddress = address;
            this.EndpointName = endpointName;
        }

        public void BindEndpointName(string name)
        {
            Util.AssistClass.StringAssist.ExceptionWhenStringEmpty(name);
            this.EndpointName = name;
        }

        public void BindEndpointAddress(EndpointPhysicalAddress address)
        {
            Util.AssistClass.ExceptionWhenNull(address);
            this.EndpointAddress = address;
        }

        public  static bool StartCheckContarct(EndpointContract contract)
        {
            if (Util.AssistClass.IsNull(contract)) { return true; } // if contract is null then neglect this check process and return check success
            var result =  contract.InvokeAll().Aggregate( (x, y) =>x && y);
            return result;
        }

        public static EndpointPhysicalAddress DefaultAddress => new EndpointPhysicalAddress("127.0.0.1", "localhost", 80, null);


        protected virtual string CreateIdentity()
        {
            return Guid.NewGuid().ToString();
        }



    }
}

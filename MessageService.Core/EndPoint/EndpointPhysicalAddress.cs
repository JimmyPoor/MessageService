using System;

namespace MessageService.Core.EndPoint
{
    public class EndpointPhysicalAddress
    {
        public string IP { get; private set; }
        public string Host { get; private set; }
        public int Port { get; private set; }
        public Uri Uri { get; private set; }

        public EndpointPhysicalAddress() { }
        public EndpointPhysicalAddress(string ip, string host,int port, Uri uri)
        {
            this.IP = ip;
            this.Host = host;
            this.Port = port;
            this.Uri = uri;
        }
    }


    public class EndpointPhysicalAddressContract : Contract
    {

    }
}

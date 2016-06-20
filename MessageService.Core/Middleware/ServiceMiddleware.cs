using MessageService.Core.Message;
using MessageService.Core.Steps;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessageService.Core.Middleware
{
    public class ServiceMiddleware : IMiddleware
    {
        private  List<string> _sendToEnpointNames =new List<string>();
        private  List<string> _receveFromEnpointNames =new List<string>();
        public  IReadOnlyList<string> SendToEndpointNames => _sendToEnpointNames;
        public  IReadOnlyList<string> ReceveFromEnpointNames => _receveFromEnpointNames;

        public IMiddlewareBehavior Behavior { get; private set; }
        public ServiceMiddleware(IMiddlewareBehavior behavior)
        {
            Util.AssistClass.ExceptionWhenNull(behavior);
            this.Behavior = behavior;
            this.Settings=new Settings();
        }


        public bool IsWorking { get; private set; }

        public bool IsEnable { get; private set; }

        public Settings Settings { get; set; }

        public string DefaultName => "Middleware";

        public virtual void Dispose()
        {
            this.IsEnable = false;
        }

        public virtual Task Setup(Settings settings)
        {
            if (!IsEnable)
            {
                this.Settings = settings ?? new Settings();
                // set behavior or default when null
                this.Behavior = this.Behavior ?? settings.Get("MiddlewareBehavior") as IMiddlewareBehavior;
                // add current middleware to cache to replace default one
                settings.Set("Middleware", this, true);
                this.IsEnable = true;
            }
            return Task.FromResult(true);
        }

        public virtual Task Start()
        {
            IsWorking = true;
            return Task.FromResult(true);
        }

        public virtual Task Stop()
        {
            this.IsWorking = false;
            return Task.FromResult(true);
        }

        public void BindSendToEndpoint(string endpoint)
        {
            if (!_sendToEnpointNames.Contains(endpoint))
                _sendToEnpointNames.Add(endpoint);
        }

        public void BindReceveFromEndpoint(string endpoint)
        {
            if (!_receveFromEnpointNames.Contains(endpoint))
                _receveFromEnpointNames.Add(endpoint);
        }
    }
}

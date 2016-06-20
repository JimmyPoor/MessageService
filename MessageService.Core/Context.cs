using MessageService.Core.EndPoint;
using MessageService.Core.Message;
using System;

namespace MessageService.Core
{
    public class Context : ICloneable
    {
        public IObjectContainer Container { set; get; }
        public Settings Settings { set; get; }

        public Context() { }
        public Context(IObjectContainer container, Settings settings)
        {
            Util.AssistClass.ExceptionWhenNull(container);
            Util.AssistClass.ExceptionWhenNull(settings);

            this.Container = container;
            this.Settings = settings;
        }

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
    }


    public class ProcessContext : Context
    {
        public IServiceMessage CurrentMessage { get; set; }
        public ServiceEndpoint CurrentEndpoint { get; set; }
        public IMessageOperation Sender { get; set; }
        public string ReceiveFromEndpointName { get; set; }
        public string SendToEndpointName { get; set; }
        public ProcessContext() { }

        public ProcessContext(IObjectContainer container, Settings settings)
            : base(container, settings)
        {

        }

        public ProcessContext(IObjectContainer container, Settings settings, ServiceEndpoint endpoint)
          : this(container, settings)
        {
            Util.AssistClass.ExceptionWhenNull(endpoint);
            this.CurrentEndpoint = endpoint;
        }

        public ProcessContext(IObjectContainer container, Settings settings, ServiceEndpoint endpoint, IMessageOperation sender)
            : this(container, settings, endpoint)
        {
            Util.AssistClass.ExceptionWhenNull(sender);
            this.Sender = sender;
        }
        public void BindEndpointName(string endpointName)
        {
            Util.AssistClass.ExceptionWhenNull(endpointName);
            this.CurrentEndpoint.Definition.BindEndpointName(endpointName);
        }

        public static void Check(ProcessContext ctx)
        {
            var hasError = Util.AssistClass.IsNull(ctx)
                || Util.AssistClass.IsNull(ctx.Container)
                || Util.AssistClass.IsNull(ctx.CurrentEndpoint);
            if (hasError)
                throw new ApplicationException("ProcessContext check failer, plz check instance for this class");
        }

        public static ProcessContext Create(Settings settings)
        {
            var container = settings.GetContainer();
            var endpoint = settings.GetEndpoint();
            var sender = settings.GetMessageOperation();
            return new ProcessContext(container, settings, endpoint, sender);
        }
    }
}

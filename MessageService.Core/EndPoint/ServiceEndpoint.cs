using MessageService.Core.Message;
using MessageService.Core.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MessageService.Core.EndPoint
{
    public class ServiceEndpoint : IStartableComponet
    {
        public string Identity => Definition.Identity;
        public string EndpointName => Definition.EndpointName;
        public Settings Settings { get; set; }
        public bool IsWorking { get; private set; }
        public bool IsEnable { get; private set; }
        public EndpointDefinition Definition { get; protected set; }
        public EndpointContract Contract => _contract;
        public IList<string> SubscribeTargets => _subscribeTargets;

        IList<string> _subscribeTargets = new List<string>();
        IMessageLisenter messageLisenter = null;
        IMessageOperation messageOperation;
        IObjectContainer _container;
        protected EndpointContract _contract;
        ProcessContext _context => CreateContext();

        public string DefaultName => "Endpoint";

        public ServiceEndpoint(EndpointDefinition definition)
        {
            Util.AssistClass.ExceptionWhenNull(definition);
            Definition = definition;
        }

        public void BindContract(EndpointContract contract)
        {
            ThrowWhenEndpointHasBeenInitial();
            this._contract = contract;
        }

        public virtual Task Setup(Settings settings)
        {
            if (!IsEnable)
            {
                settings = settings ?? new Settings();
                _container = settings.GetContainer();

                // will check endpoint name,Identity and the other properties in EndpointDefinition, 
                var result = EndpointDefinition.StartCheckContarct(_contract);
                if (!result) throw new ApplicationException("Constract  check process failure");

                messageOperation = settings.GetMessageOperation();
                messageLisenter = settings.GetMessageListener();

                // add current endpoint to cache to replace default one
                settings.Set("Endpoint",this,true);
                this.Settings = settings;
                this.IsEnable = true;
            }
            return Task.FromResult(0);
        }
        public virtual Task Start()
        {
            ThrowWhenEndpointIsNotEnable();
            IsWorking = true;
            return Task.FromResult(0);
        }
        public Task Stop()
        {
            ThrowWhenEndpointIsNotEnable();
            IsWorking = false;
            return Task.FromResult(0);
        }
        public void Dispose()
        {
            this.IsEnable = false;
        }
        public Task SendServiceMessage<T>(string target, Func<T> messageFactory, Action<ProcessContext> callback = null) where T : IServiceMessage
        {
            AssistClass.ExceptionWhenNull(messageFactory);
            return this.Send(target, messageFactory(), callback);
        }

        public Task Send(string target, object message, Action<ProcessContext> callback = null)
        {
            ThrowWhenEndpointIsNotEnable();
            ThrowWhenEndpointIsNotWorking();
            return this.messageOperation.Send(target, message, _context, callback);
        }

        public Task Publish(object message, Action<ProcessContext> callback = null)
        {
            ThrowWhenEndpointIsNotEnable();
            ThrowWhenEndpointIsNotWorking();
            return this.messageOperation.Publish(this.SubscribeTargets.ToArray(), message, _context, callback);
        }
        
        public void Subscribe(string subTarget)
        {
            if (Util.AssistClass.StringAssist.IsNullOrEmpty(subTarget)) return;
            _subscribeTargets.Add(subTarget);
        }

        public Task Listen()
        {
            ThrowWhenEndpointIsNotEnable();
            ThrowWhenEndpointIsNotWorking();
            this.messageLisenter.Listen(CreateContext());
            return Task.FromResult(0);
        }

        protected virtual ProcessContext CreateContext()
        {
            return ProcessContext.Create(Settings);
        }

        private void ThrowWhenEndpointIsNotEnable()
        {
            if (!IsEnable) throw new ApplicationException("Endponit has not been set up or endpoint has been closed");
        }

        private void ThrowWhenEndpointHasBeenInitial()
        {
            if (IsEnable) throw new ApplicationException("plz use this method befor endpoint set up");
        }

        private void ThrowWhenEndpointIsNotWorking()
        {
            if (!IsWorking) throw new ApplicationException("must start up endpoint first");
        }
    }
}

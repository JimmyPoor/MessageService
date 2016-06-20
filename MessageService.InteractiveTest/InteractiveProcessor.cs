using MessageService.Core;
using MessageService.Core.Bus;
using MessageService.Core.EndPoint;
using MessageService.Core.Message;
using System;
using System.Collections.Generic;
using Util = MessageService.Core.Util;

namespace MessageService.InteractiveTest
{
    public class InteractiveProcessor<TContext> where TContext : InteractiveContext
    {
        List<IMessageLisenter> _receivers = new List<IMessageLisenter>();
        InteractiveScenario _scenario;
        Action<ServiceBus, TContext> _action;
        Action<TContext> _done;
        ServiceBus _bus = new ServiceBus();
        Settings settings = new Settings();
        Action<object, ReceivingErrorEventArgs> _receiveError;
        Action<object, SendErrorEventArgs> _sendError;
        public InteractiveProcessor<TContext> BindEndpoint(ServiceEndpoint endpoint)
        {
            _bus.WithEndpoint(endpoint.Definition);
            return this;
        }

        public InteractiveProcessor<TContext> AddCompontents(IComponent component)
        {
            _bus.AddOtherComponent(component);
            return this;
        }

        public InteractiveProcessor<TContext> AddReceiver(IMessageLisenter receiver)
        {
            this._receivers.Add(receiver);
            return this;
        }

        public InteractiveProcessor<TContext> DefinitionTestScenario(InteractiveScenario scenario)
        {
            _scenario = scenario;
            return this;
        }

        public InteractiveProcessor<TContext> BindSendAction(Action<ServiceBus, TContext> action)
        {
            this._action = action;
            return this;
        }

        public InteractiveProcessor<TContext> End(Action<TContext> done)
        {
            this._done = done;
            return this;
        }

        public InteractiveProcessor<TContext> BindSendErrorHandler(Action<object, SendErrorEventArgs> whenError)
        {
            this._sendError = whenError;
            return this;
        }

        public InteractiveProcessor<TContext> BindReceiveErrorHandler(Action<object, ReceivingErrorEventArgs> whenError)
        {
            this._receiveError = whenError;
            return this;
        }

        public async void Start()
        {
            _bus.Initial(settings);
            var processContext = ProcessContext.Create(settings);
            var sender = settings.GetMessageOperation();
            sender.OnErrorWhenMessageOperation += (o, e) =>
            {
                if (_sendError != null)
                    _sendError(o, e);
            };
            if (_receivers.Count <= 0)
            {
                _receivers.Add(settings.GetMessageListener());
            }
            foreach (var receiver in _receivers)
            {
                var _receiver = receiver ?? settings.GetMessageListener();
                if (!_receiver.IsEnable)
                {
                    await _receiver.Setup(settings);
                }
                _receiver.OnErrorWhenReceiving += (o, e) =>
                {
                    if (_receiveError != null)
                        _receiveError(o, e);
                };
                await _receiver.Listen(processContext, (processCtx) =>
                {
                    var receiveCtx = ContextMapping(processCtx);
                    if (!Util.AssistClass.IsNull(_done) && !Util.AssistClass.IsNull(receiveCtx))
                    {
                        receiveCtx._currentBus = _bus;
                        _done(receiveCtx);
                    }
                });
            }
            var sendCtx = ContextMapping(processContext);
            await _bus.Start();
            if (_action != null)
                _action.Invoke(_bus, sendCtx);
        }

        private TContext ContextMapping(ProcessContext processContext)
        {
            var processCtx = (ProcessContext)processContext.Clone();
            var ctx = Activator.CreateInstance<TContext>();
            ctx.CurrentMessage = processContext.CurrentMessage;
            ctx.CurrentEndpoint = processContext.CurrentEndpoint;
            ctx.Settings = processContext.Settings;
            return ctx;
        }
    }

    [Serializable]
    public class InteractiveMessage : ServiceMessage
    {
        public InteractiveMessage(byte[] body, Settings headers) : base(body, headers) { }
    }

    [Serializable]
    public class InteractiveReplyMessage : ServiceReplyMessage
    {
        public InteractiveReplyMessage(byte[] body, Settings headers)
            : base(body, headers)
        {

        }
    }

    [Serializable]
    public class InteractiveSubscribeMessage : ServiceSubscribeMessage
    {
        public InteractiveSubscribeMessage(string subTarget, byte[] body, Settings headers)
            : base(subTarget, body, headers)
        {

        }
    }

    public class InteractiveContext : ProcessContext
    {
        public ServiceBus _currentBus;

        public void CloseTest()
        {
            if (!Util.AssistClass.IsNull(_currentBus) && !_currentBus.IsWorking)
            {
                _currentBus.Stop();
                _currentBus.Dispose();
            }
        }
    }

    public class InteractiveScenario
    {
        public string Name { get; set; }
        public string Definition { get; set; }
    }

}

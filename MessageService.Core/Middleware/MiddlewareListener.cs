using MessageService.Core.Message;
using MessageService.Core.Steps;
using MessageService.Core.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static MessageService.Core.Util.AssistClass;

namespace MessageService.Core.Middleware
{
    public class MiddlewareLisenter : IMessageLisenter
    {
        IMiddleware _middle;
        IList<IncomingStep> _currentIncomingSteps = new List<IncomingStep>();
        IList<IncomingStep> _tempSteps= new  List<IncomingStep>();

        protected  string[] DefaultStepsName = {
             typeof(Steps.DeserializeMessageStep).Name,
             typeof(Steps.SubscribeMessageStep).Name 
        };

        public event ReceivingErrorHandler OnErrorWhenReceiving;
        public Settings Settings { get; set; }
        public bool IsEnable { get; protected set; }

        public string DefaultName => "MessageLisener";

        public MiddlewareLisenter(IMiddleware middle)
        {
            this._middle = middle;
        }

        public MiddlewareLisenter() { }

        public void AddIncomingStep(string stepName)
        {
            var step = StepsManager.GetStepFromCache(stepName, StepsManager.IncomingSteps) as IncomingStep;
            if (AssistClass.IsNull(step)) return;
            if (IsEnable)
                _currentIncomingSteps.Add(step);
            else
                _tempSteps.Add(step);
        }

        public virtual Task Setup(Settings settings = null)
        {
            if (!IsEnable)
            {
                settings = settings ?? new Settings();
                _middle = _middle ?? settings.GetMiddleware();
                var assemblies = AssemblyAssist.CurrentDomainAssemblies;
                //initial steps
                 StepsManager.BindIncomingSteps(assemblies);
                //bind messageInvokers
                MessageInvokerManager.BindInvokers(assemblies);
                //build steps
                BuildIncomingSteps();
                this.Settings = settings;
                this.IsEnable = true;
            }
            return Task.FromResult(0);
        }

        public  Task Listen(ProcessContext ctx, Action<ProcessContext> callback = null)
        {
            if (!IsEnable) throw new ApplicationException("must setup listener first");
            ProcessContext.Check(ctx);
            var queue = ctx.CurrentEndpoint.EndpointName;
            if (Util.AssistClass.IsNull(queue)
                || Util.AssistClass.StringAssist.IsNullOrEmpty(queue)
                || queue == Util.StaticStringDefinition.DEFAULT_ENDPOINT_NAME)
            {
                throw new NullReferenceException("listen queue name can't be null, you need use ctx.BindEndpointName in logic first");
            }
            return _middle.Behavior.Listen(queue, ctx, WhenReceive, callback);
        }

        private async void WhenReceive(Task<byte[]> receiveBytesTask, ProcessContext context, Action<ProcessContext> callback)
        {
            var ctx = context.Clone() as ProcessContext; // in order to create  contenxt clone object which deal with each request (new thread)
            ctx.CurrentMessage = new ServiceMessage(receiveBytesTask.Result, ctx.Settings); // wrape receive bytes to service message

            await StepsManager.InvokeSteps(ctx, _currentIncomingSteps)
             .ContinueWith((t) =>
             {
                 var stepctx = t.Result;
                 var stepErrors = stepctx.Errors;
                 if (stepErrors.Count > 0 && OnErrorWhenReceiving!=null)
                 {
                     var recevingError = new ReceivingError(null, "Step error", stepErrors);
                    OnErrorWhenReceiving.Invoke(this, new ReceivingErrorEventArgs(new List<ReceivingError> { recevingError }));
                 }
                 var processCtx = stepctx.InnerContext;
                 var msg = processCtx.CurrentMessage; // this current message may be change in steps
                 var fromEndpointName = msg.Headers.Get<string>(StaticStringDefinition.FROM_ENDPOINT);
                 if (!Util.AssistClass.StringAssist.IsNullOrEmpty(fromEndpointName))
                 {
                     processCtx.ReceiveFromEndpointName = fromEndpointName;
                     _middle.BindReceveFromEndpoint(fromEndpointName);
                 }

                if (!Util.AssistClass.IsNull(msg))
                 {
                     var mappingAction = new InvokeContextMapper().Mapping;
                     MessageInvokerManager.FireInvokerByMessage(msg, processCtx, mappingAction)
                     .ContinueWith((task)=> {
                         if (!Util.AssistClass.IsNull(callback))
                         {
                             callback(processCtx);
                         }
                     });
                 }
             });
        }

        private  void BuildIncomingSteps()
        {
            StepsManager.FindStepsByNamesFromCacheAndAddToList(_currentIncomingSteps, DefaultStepsName, StepsManager.IncomingSteps);

            foreach(var step in _tempSteps)
            {
                if (!_currentIncomingSteps.Contains(step))
                {
                    _currentIncomingSteps.Add(step);
                }
            }
        }

        public void Dispose()
        {
            this.IsEnable = false;
        }
    }
}

using MessageService.Core.Message;
using MessageService.Core.Steps;
using MessageService.Core.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessageService.Core.Middleware
{
    public class MiddlewareSender : IMessageOperation
    {
        IMiddleware _middle;
        IList<IOutgoingStep> _currentOutgoingSteps = new List<IOutgoingStep>();
        IList<IOutgoingStep> _tempSteps = new List<IOutgoingStep>(); // tempSteps only for insert customer steps  before sender setup
        protected string[] DefaultStepsName =
        {
             typeof(Steps.SerializeMessageStep).Name
        };
        public event OperationErrorHandler OnErrorWhenMessageOperation;
        public Settings Settings { get; set; }
        public bool IsEnable { get; protected set; }

        IEnumerable<IOutgoingStep> _cache => StepsManager.OutgoingSteps;

        public string DefaultName => "MessageOperation";

        public MiddlewareSender(IMiddleware middle)
        {
            this._middle = middle;
        }

        public MiddlewareSender() { }

        public virtual Task Setup(Settings settings = null)
        {
            if (!IsEnable)
            {
                _middle = _middle ?? settings.GetMiddleware();
                //initila steps
                var assemblies = Util.AssistClass.AssemblyAssist.CurrentDomainAssemblies;
                StepsManager.BindOutgoingSteps(assemblies);
                //build steps
                BuildOutgoingSteps();
                this.Settings = settings;
                IsEnable = true;
            }
            return Task.FromResult(0);
        }

        public void Dispose()
        {
            this.IsEnable = false;
        }

        public void AddOutgoingSteps(string stepName)
        {
            var step = StepsManager.GetStepFromCache(stepName, _cache);
            if (IsEnable)
                _currentOutgoingSteps.Add(step);
            else
                _tempSteps.Add(step);
        }

        public Task Send(string target, object msg, ProcessContext ctx, Action<ProcessContext> callback)
        {
            if (!IsEnable) throw new ApplicationException("must setup sender first");
            ProcessContext.Check(ctx);

            Util.AssistClass.ExceptionWhenNull(_middle);
            Util.AssistClass.ExceptionWhenNull(msg);

            var ctxCopy = ctx.Clone() as ProcessContext;
            ctxCopy.CurrentMessage = ServiceMessage.CreateMessage(msg, ctxCopy.Settings);
            ctxCopy.SendToEndpointName = target;

            var currentEndpointName = ctxCopy.CurrentEndpoint.EndpointName;
            if (!Util.AssistClass.IsNull(currentEndpointName))
                ctxCopy.CurrentMessage.Headers.Set(StaticStringDefinition.FROM_ENDPOINT, currentEndpointName);
            //set up and invoke steps
            return StepsManager.InvokeSteps(ctxCopy, _currentOutgoingSteps)
                .ContinueWith(SendMessageAfterStepsRunning)
                .ContinueWith((t) =>
                {
                    if (!Util.AssistClass.IsNull(callback)) callback(ctxCopy);
                })
                ;
        }

        public Task SendServiceMessage(string target, IServiceMessage message, ProcessContext ctx, Action<ProcessContext> callback)
        {
            return this.Send(target, message, ctx, callback);
        }

        public Task Subscribe(string target, ISubscribeMessage subMessage, ProcessContext ctx, Action<ProcessContext> callback)
        {
            Util.AssistClass.ExceptionWhenNull(subMessage);
            Util.AssistClass.StringAssist.ExceptionWhenStringEmpty(subMessage.SubscribeTarget);
            return this.Send(target, subMessage, ctx, callback);
        }

        public Task Publish(string[] subTargets, object message, ProcessContext ctx, Action<ProcessContext> callback)
        {
            List<Task> sendAction = new List<Task>();
            Util.AssistClass.ExceptionWhenNull(subTargets);
            foreach (var subTarget in subTargets)
            {
                sendAction.Add(
                    this.Send(subTarget, message, ctx, null)
                    );
            }
            if (sendAction.Count > 0)
            {
                Task.WaitAll(sendAction.ToArray());
            }
            if (!Util.AssistClass.IsNull(callback))
            {
                callback(ctx);
            }
            return Task.FromResult(0);
        }

        public Task Reply(IReplyMessage msg, ProcessContext ctx, Action<ProcessContext> callback)
        {
            Util.AssistClass.ExceptionWhenNull(msg);
            Util.AssistClass.ExceptionWhenNull(msg.ReplyTarget);
            return this.Send(msg.ReplyTarget, msg, ctx, callback);
        }

        private async void SendMessageAfterStepsRunning(Task<StepProcessContext> task)
        {
            var stepContext = task.Result;
            var errors = stepContext.Errors;
            if (errors.Count > 0 && OnErrorWhenMessageOperation != null)
            {
                var sendError = new SendError(null, "send steps error", errors, "send");
                var args = new SendErrorEventArgs(new List<SendError> { sendError });
                OnErrorWhenMessageOperation.Invoke(this, args);
            }
            var processContext = stepContext.InnerContext;
            var currentTarget = processContext.SendToEndpointName;
            byte[] serviceMessageBytes = processContext.CurrentMessage.Body;

            if (!Util.AssistClass.IsNull(_middle)
            && !Util.AssistClass.IsNull(serviceMessageBytes)
            && !Util.AssistClass.IsNull(currentTarget)
            )
            {
                var currentTargetString = currentTarget.ToString();
                _middle.BindSendToEndpoint(currentTargetString);
                await _middle.Behavior.Send(currentTargetString, serviceMessageBytes);
            }
        }

        private void BuildOutgoingSteps()
        {
            //add customer steps  before lisenter setup
            foreach (var step in _tempSteps)
            {
                if (!_currentOutgoingSteps.Contains(step))
                {
                    _currentOutgoingSteps.Add(step);
                }
            }

            //default step should be after than customer steps in sender
            StepsManager.FindStepsByNamesFromCacheAndAddToList(_currentOutgoingSteps, DefaultStepsName, _cache);
        }

    }
}

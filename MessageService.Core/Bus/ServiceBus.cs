using MessageService.Core.Enctryption;
using MessageService.Core.EndPoint;
using MessageService.Core.Message;
using MessageService.Core.Middleware;
using MessageService.Core.Persistance;
using MessageService.Core.Serializor;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;

namespace MessageService.Core.Bus
{
    public class ServiceBus : IServiceBus
    {
       
        Settings _settings;
        IObjectContainer _container;
        ServiceEndpoint _currentEndPoint;
        IList<IComponent> _components = new List<IComponent>();
        public bool IsWorking { get; private set; }
        public IList<IComponent> Components => _components;
        bool _isInitialed;

        public void Initial(Settings settings = null)
        {
            settings = settings ?? new Settings();
            if (_isInitialed)
                return;

            if (Util.AssistClass.IsNull(_currentEndPoint))
            {
                _currentEndPoint = settings.GetEndpoint();
                _currentEndPoint.Setup(settings);
                settings.Set(_currentEndPoint.DefaultName, _currentEndPoint);
            }
            //initial other compontents
            foreach (var compontent in _components)
            {
                compontent.Setup(settings);
                settings.Set(compontent.DefaultName, compontent);
            }
            //ioc
            _container = _container ?? settings.GetContainer();
            ComponentsInjection(_container);
            settings.Set("Container", _container);

            _settings = settings;
            //end
            _isInitialed = true;
        }

        public Task Send(string target, object message)
        {
            EnsureBusIsInitial();
            return _currentEndPoint.Send(target, message);
        }


        public async Task Start()
        {
            EnsureBusIsInitial();
            await _currentEndPoint.Start().ContinueWith((t) =>
            {
                this.StartComponents();
                this.IsWorking = true;
            });

        }

        public Task Stop()
        {
            return _currentEndPoint.Stop().ContinueWith((t) =>
           {
               this.StopComponents();
               this.IsWorking = false;
           });

        }

        public void Dispose()
        {
            if (IsWorking)
                this.Stop().ContinueWith(x =>
                {
                    _currentEndPoint.Dispose();
                    DisposeComponents();
                });
            else
            {
                _currentEndPoint.Dispose();
                DisposeComponents();
            }
        }


        public ServiceBus BindContainer(IObjectContainer container)
        {
            EnsureBusIsNotInitial();
            Util.AssistClass.ExceptionWhenNull(container);
            this._container = container;
            return this;
        }

        public ServiceBus WithEndpoint(EndpointDefinition endpointDefinition)
        {
            EnsureBusIsNotInitial();
            Util.AssistClass.ExceptionWhenNull(endpointDefinition);
            _currentEndPoint = new ServiceEndpoint(endpointDefinition);
            this._components.Add(_currentEndPoint);
            return this;
        }

        public ServiceBus UseContainer(IObjectContainer container)
        {
            Util.AssistClass.ExceptionWhenNull(container);
            this._container = container;
            return this;
        }

        public ServiceBus BindMiddelwareBehavior(IMiddlewareBehavior behavior)
        {
            EnsureBusIsNotInitial();
            Util.AssistClass.ExceptionWhenNull(behavior);
            this._components.Add(behavior);
            return this;
        }

        public ServiceBus BindMessageListener(IMessageLisenter listener)
        {
            EnsureBusIsNotInitial();
            Util.AssistClass.ExceptionWhenNull(listener);
            this._components.Add(listener);
            return this;
        }

        public ServiceBus BindMessageOperation(IMessageOperation operation)
        {
            EnsureBusIsNotInitial();
            Util.AssistClass.ExceptionWhenNull(operation);
            this._components.Add(operation);
            return this;
        }

        public ServiceBus BindMessageSerilizer(IMessageSerilizer serilizer)
        {
            EnsureBusIsNotInitial();
            Util.AssistClass.ExceptionWhenNull(serilizer);
            this._components.Add(serilizer);
            return this;
        }

        public ServiceBus UseMiddleware(IMiddleware middleware)
        {
            EnsureBusIsNotInitial();
            Util.AssistClass.ExceptionWhenNull(middleware);
            this._components.Add(middleware);
            return this;
        }

        public ServiceBus AddOtherComponent(IComponent component)
        {
            EnsureBusIsNotInitial();
            Util.AssistClass.ExceptionWhenNull(component);
            Util.AssistClass.StringAssist.ExceptionWhenStringEmpty(component.DefaultName);
            this._components.Add(component);
            return this;
        }

        private void StartComponents()
        {
            DoSomthingForComponents((component) =>
            {
                if (component is IStartableComponet)
                {
                    (component as IStartableComponet).Start();
                }
            });
        }

        private void StopComponents()
        {
            DoSomthingForComponents((component) =>
            {
                if (component is IStartableComponet)
                {
                    (component as IStartableComponet).Stop();
                }
            });

        }

        private void DisposeComponents()
        {
            DoSomthingForComponents((component) =>
            {
                 component .Dispose();
            });
        }

        private void DoSomthingForComponents(Action<IComponent> componentAction)
        {
            foreach (var component in _components)
            {
                componentAction(component);
            }
        }

        private void ComponentsInjection(IObjectContainer container)
        {
            Util.AssistClass.ExceptionWhenNull(container);
            ComponentsRelfectInjection(container);
        }

        private void EnsureBusIsInitial()
        {
            if (!_isInitialed) throw new ApplicationException("must invoke intial method first");
        }

        private void EnsureBusIsNotInitial()
        {
            if (_isInitialed) throw new ApplicationException("plz set this method befor bus initial been invoked");
        }

        private void ComponentsRelfectInjection(IObjectContainer container)
        {
            MethodInfo[] ms = container.GetType().GetMethods();
            foreach (MethodInfo info in ms)
            {

                if (info.Name == "RegistInstance" && info.IsGenericMethod)
                {
                    foreach (var component in _components)
                    {
                        var interfaceTypes = ((TypeInfo)component.GetType()).ImplementedInterfaces;
                        if (interfaceTypes == null || interfaceTypes.Count() == 0)
                            continue;
                        info.MakeGenericMethod(new Type[] { interfaceTypes.First() })
                            .Invoke(container, new object[] { component, LifeCycleEnum.Singleton });
                    }
                    break;
                }
            }
        }


    }
}

using Microsoft.Practices.Unity;
using System;
using System.Linq;
using LifetimeManager = Microsoft.Practices.Unity.ContainerControlledLifetimeManager;
using UnityInnerContainer = Microsoft.Practices.Unity.UnityContainer;

namespace MessageService.Core.Container.Unity
{
    public class UnityContainer : IObjectContainer
    {
        UnityInnerContainer _container = new UnityInnerContainer();

        public void Regist<From, To>(LifeCycleEnum lifecycle) where To : From
        {
            var lifetimeManager = Mapping(lifecycle);
            _container.RegisterType<From, To>(lifetimeManager);
        }

        public void RegistInstance<T>(T instance, LifeCycleEnum lifecycle)
        {
            Util.AssistClass.ExceptionWhenNull(instance);
            var lifetimeManager = Mapping(lifecycle);
            _container.RegisterInstance<T>(instance, lifetimeManager);
        }

        public T Resolve<T>(params Tuple<string, object>[] injection) where T : class
        {
            if (!_container.IsRegistered<T>()) { return null; }
            ParameterOverride[] paras = null;
            if (!Util.AssistClass.IsNull(injection) && Util.AssistClass.ArrayAssist.HasElements(injection))
            {
                var existsPara = injection.Where(para => !Util.AssistClass.IsNull(para.Item2));
                if (existsPara != null && existsPara.Count() > 0)
                {
                    paras = injection
                        .Select(para => new ParameterOverride(para.Item1, para.Item2))
                        .ToArray();
                }
            }
    
            var result = Util.AssistClass.IsNull(paras) ?
                _container.Resolve<T>() :
                _container.Resolve<T>(paras);
            
            return result;
        }

        public bool IsRegist<T>()
        {
            return _container.IsRegistered<T>();
        }

        private LifetimeManager Mapping(LifeCycleEnum lifecycle)
        {
            switch (lifecycle)
            {
                case LifeCycleEnum.Singleton: return new ContainerControlledLifetimeManager();
                default: return new ContainerControlledLifetimeManager();

            }
        }

       
    }
}

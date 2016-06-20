using System;

namespace MessageService.Core
{
    /// <summary>
    /// this interface for IOC container which will implement by some IOC tools like Unity, Autofac ,etc  
    /// will add constructor DI in regist aciton later
    /// </summary>
    public interface IObjectContainer
    {
        void RegistInstance<T>(T instance, LifeCycleEnum lifecycle);
        void Regist<From, To>(LifeCycleEnum lifecycle) where To : From;
        T Resolve<T>(params Tuple<string,object>[] injection) where T : class;
        bool IsRegist<T>();
    }


    public enum LifeCycleEnum
    {
         Singleton
    }
}

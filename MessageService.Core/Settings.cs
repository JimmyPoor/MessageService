using MessageService.Core.Container.Unity;
using MessageService.Core.EndPoint;
using MessageService.Core.Util;
using System;
using System.Collections.Generic;

namespace MessageService.Core
{
    [Serializable]
    public class Settings
    {
        IDictionary<string, object> _caches = new Dictionary<string, object>();
        public virtual void Set<V>(string k, V v)
        {
            Set(k, v as object,false);
        }

        public virtual void Set(string k, object o, bool replace = false)
        {
            AssistClass.StringAssist.ExceptionWhenStringEmpty(k);
            AssistClass.ExceptionWhenNull(o);

            if (!_caches.ContainsKey(k))
            {
                _caches.Add(k, o);
            }
            else if(replace)
            {
                _caches[k] = o;
            }

        }

        public virtual object Get(string k)
        {
            if (_caches.ContainsKey(k))
            {
                return _caches[k];
            }
            return null;
        }

        public virtual V Get<V>(string k)
        {
            var o = Get(k);
            return (V)o;
        }
        public virtual int ItemCount
        {
            get
            {
                return _caches == null ? 0 : _caches.Count;
            }
        }

        public virtual object this[string k]
        {
            get
            {
                return Get(k);
            }
        }
    }

    public static class SettingsExtension
    {
        public static IObjectContainer GetContainer(this Settings settings)
        {
            settings = settings ?? new Settings();
            var container = settings["Container"] as IObjectContainer ?? new UnityContainer();
            settings.Set("Container", container);
            return container;
        }

        public static Middleware.IMiddlewareBehavior GetMiddlewareBehavior(this Settings settings)
        {
            var defaultBehaivor = new Middleware.RabbiMQ.RabbitMQMiddlewareBehavior();
            return GetInstance(settings, "MiddlewareBehavior", defaultBehaivor);
        }

        public static Middleware.IMiddleware GetMiddleware(this Settings settings)
        {
            var behavior = settings.GetMiddlewareBehavior();
            var defultMiddleware = new Middleware.ServiceMiddleware(behavior);
            return GetInstance(settings, "Middleware",defultMiddleware);
        }

        public static EndPoint.EndpointDefinition GetEndpointDefinition(this Settings settings)
        {
            return GetInstance(settings, "EndpointDefinition", new EndpointDefinition());
        }

        public static EndPoint.ServiceEndpoint GetEndpoint(this Settings settings)
        {
            var definition = settings.GetEndpointDefinition();
            var defaultEndpoint = new ServiceEndpoint(definition);
            return GetInstance(settings, "Endpoint", defaultEndpoint);
        }

        public static Message.IMessageLisenter GetMessageListener(this Settings settings)
        {
            return GetInstance(settings, "MessageLisener", new Middleware.MiddlewareLisenter());
        }

        public static Message.IMessageOperation GetMessageOperation(this Settings settings)
        {
            return GetInstance(settings, "MessageOperation", new Middleware.MiddlewareSender());
        }

        public static Serializor.IMessageSerilizer GetSerilizerFromSettings(this Settings settings)
        {
            return GetInstance(settings, "MessageSerilizer", new Serializor.DefaultSerilizer());
        }

        public static Persistance.IPersistence GetPersistance(this Settings settings)
        {
            return GetInstance(settings, "Persistance", new Persistance.ServicePersistence());
        }

        public static T GetInstance<T>(Settings settings,string name,T _default) where T :class
        {
            var container = settings.GetContainer();
            var instance = container.Resolve<T>();
            instance = instance ?? settings[name] as T ?? _default;
            if(instance is IComponent)
            {
                (instance as IComponent).Setup(settings);
            }
            settings.Set(name, instance);
            return instance;
        }

    }
}

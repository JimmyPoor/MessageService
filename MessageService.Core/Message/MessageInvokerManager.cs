using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MessageService.Core.Message
{
    public static class MessageInvokerManager
    {
        static ConcurrentDictionary<Type, Tuple<Type, Object>> _invokers = new ConcurrentDictionary<Type, Tuple<Type, object>>();
        
        public static Task BindInvokers(params Assembly[] assemblies)
        {
            Func<Type, bool> isInvokerInterface = (type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IMessageInvoker<>));

            if (!Util.AssistClass.IsNull(assemblies))
            {
                var invokerTypes = assemblies.SelectMany(
                     assembly=> assembly.GetTypes().Where(type=> type.GetInterfaces()
                       .Any(isInvokerInterface)
                    ) );
                foreach(var type in invokerTypes)
                {
                    var invoker = (Activator.CreateInstance(type));
                    foreach (var interfaceType in type.GetInterfaces())
                    {
                        if (isInvokerInterface.Invoke(interfaceType))
                        {
                            var messagType = interfaceType.GetGenericArguments()[0];
                            _invokers.TryAdd(messagType, new Tuple<Type, object>(invoker.GetType(), invoker));
                        }
                    }
                
                }
            }
            return Task.FromResult(0);
        }
        public static Task FireInvokerByMessage(IServiceMessage message, ProcessContext ctx, Func<ProcessContext, MessageInvokeContext> mapper)
        {
            Util.AssistClass.ExceptionWhenNull(message);
            Util.AssistClass.ExceptionWhenNull(mapper);
            Util.AssistClass.ExceptionWhenNull(ctx);

            var messageType = message.GetType();
            if (_invokers.ContainsKey(messageType))
            {
                var tuple = _invokers[messageType];
                var context=mapper(ctx);
                return Task.Run(() => {
                    tuple.Item1.InvokeMember("Invoke", BindingFlags.Default | BindingFlags.InvokeMethod, null, tuple.Item2, new object[] { message, context });
                });
            }
            return Task.FromResult(0);
        }
        public static int InvokerCount => _invokers.Count;

        public static Type GetInvokerTypeByMessageType(Type messageType)
        {
            if (_invokers.ContainsKey(messageType))
            {
               return  _invokers[messageType].Item1;
            }
            return null;
        }
    }
}

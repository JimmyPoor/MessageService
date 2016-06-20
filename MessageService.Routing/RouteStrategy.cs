using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageService.Routing
{
    public interface IRouteStrategy
    {
        string StrategyName { get; }
        void AddInnerStrategy(Func<object, bool> condiftion, Func<RouteContext, RouteResult> strategy);
        Task<IEnumerable<RouteResult>> Invoke(object message, RouteContext ctx);
        int InnerStrategyCount { get; }
        bool IsEnable { get; }
        void Disable();
        void Enable();
    }

    public class DefaultRouteStrategy : IRouteStrategy
    {
        public virtual string StrategyName { get; protected set; }

        IList<KeyValuePair<Func<object, bool>, Func<RouteContext, RouteResult>>> SubStrategies { get; set; }

        public int InnerStrategyCount => SubStrategies.Count;

        public bool IsEnable { get; private set; }

        public DefaultRouteStrategy(string name)
        {
            this.StrategyName = name;
            SubStrategies = new List<KeyValuePair<Func<object, bool>, Func<RouteContext, RouteResult>>>();
            Enable();
        }


        public virtual void AddInnerStrategy(Func<object, bool> condiftion, Func<RouteContext, RouteResult> strategy)
        {
            Core.Util.AssistClass.ExceptionWhenNull(strategy);
            var inner = new KeyValuePair<Func<object, bool>, Func<RouteContext, RouteResult>>(condiftion, strategy);
            SubStrategies.Add(inner);
        }

        public virtual Task<IEnumerable<RouteResult>> Invoke(object message, RouteContext ctx)
        {
            Core.Util.AssistClass.ExceptionWhenNull(message);
            Core.Util.AssistClass.ExceptionWhenNull(ctx);

            var matched = SubStrategies.Where(x => x.Key.Invoke(message));
            IEnumerable<RouteResult> results = null;
            if (matched != null )
            {
                results = matched.Select(x => x.Value.Invoke(ctx));
            }
            return Task.FromResult(results);

        }

        public virtual void Disable()
        {
            IsEnable = false;
        }

        public virtual void Enable()
        {
            IsEnable = true;
        }
    }

}

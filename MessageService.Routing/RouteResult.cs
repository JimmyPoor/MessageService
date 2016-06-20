using System.Collections.Generic;

namespace MessageService.Routing
{
    public class RouteResult
    {
        public IList<RouteError> RouteErrors { get; private set; }

        public IList<EndpointTarget> Targets { get; private set; }

        public RouteResult()
        {
            Targets = new List<EndpointTarget>();
            RouteErrors = new List<RouteError>();
        }

        public bool HasRoute => Targets.Count > 0;

        public bool HasRouteError => RouteErrors.Count > 0;


        public void AddTarget(EndpointTarget target)
        {
            if (!Core.Util.AssistClass.IsNull(target) && !Targets.Contains(target))
            {
                Targets.Add(target);
            }
        }

        public void AddError(RouteError err)
        {
            if (!Core.Util.AssistClass.IsNull(err) && !RouteErrors.Contains(err))
            {
                RouteErrors.Add(err);
            }
        }



    }


    public class EndpointTarget
    {
        string _endpointTarget;


         public EndpointTarget(string target)
        {
            _endpointTarget = target;
        }

        public override string ToString()
        {
            return this._endpointTarget;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, this))
            {
                return true;
            }
            else if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return Equals(obj as EndpointTarget);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

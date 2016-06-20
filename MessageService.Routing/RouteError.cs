using MessageService.Core;
using System;

namespace MessageService.Routing
{
    public class RouteError : Error
    {
          public RouteError(Exception inner, string error)
            : base(inner, error)
        {

        }

        public override string ToString()
        {
            return this.ErrorMessage;
        }

    }
}

using System;

namespace MessageService.Core.Util
{
    public interface IObjectMapper<From ,To>
    {
         Func<From, To> Mapping { get; }

    }
}

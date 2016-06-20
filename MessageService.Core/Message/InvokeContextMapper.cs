using MessageService.Core.Util;
using System;

namespace MessageService.Core.Message
{
    public class InvokeContextMapper : IObjectMapper<ProcessContext, MessageInvokeContext>
    {
        public Func<ProcessContext, MessageInvokeContext> Mapping => spc => Map(spc);

        protected virtual MessageInvokeContext Map(ProcessContext spctx)
        {
            AssistClass.ExceptionWhenNull(spctx);
            return new MessageInvokeContext() { InnerContext = spctx };
        }
    }
}

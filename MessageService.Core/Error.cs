using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Core
{
    public delegate void ErrorHandler<Err>(object sender, ErrorEventArgs<Err> args) where Err : Error;
    public class ErrorEventArgs<Err>: EventArgs where Err:Error
    {
        public List<Err> Errors { get; private set; }
        public ErrorEventArgs(List<Err> errors)
        {
            this.Errors = errors;
        }
    }

    public class Error
    {
        public object InnerErrors { get; private set; }
        public Type ErrorType => this.GetType();
        public Exception InnerException { get; private set; }
        public string ErrorMessage { get; private set; }

        public Error(Exception innerExcepiton, string errorMessage)
            :this(innerExcepiton,errorMessage,null)
        {

        }
        public Error(Exception innerExcepiton, string errorMessage,object innerErrors)
        {
            this.InnerException = innerExcepiton;
            this.ErrorMessage = errorMessage;
            this.InnerErrors=innerErrors;
        }
    }
}

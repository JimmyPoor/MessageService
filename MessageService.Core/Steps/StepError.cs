using System;

namespace MessageService.Core.Steps
{
    public class StepError: Error
    {
     
        public IStep ErrorStep { get; private set; }

        public StepError(Exception innerException,string error,IStep errorStep)
            :base(innerException, error)
        {
            this.ErrorStep = errorStep;
        }

        public override string ToString()
        {
            return string.Format(" error step:{0}\n error tyep:{1}\n error message:{2}", this.ErrorStep, this.ErrorType,this.ErrorMessage);
        }
    }

}

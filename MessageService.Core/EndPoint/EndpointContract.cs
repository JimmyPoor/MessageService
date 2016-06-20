using System;
using System.Text.RegularExpressions;

namespace MessageService.Core.EndPoint
{
    /// <summary>
    /// check endpoint  property's value
    /// </summary>
    public class EndpointContract : Contract
    {
        string _endpointNameNotAllowedRegex = "([^.!&*()-+=#`{}]*)";
        public void AddEndpointNameContract(string endpointName)
        {
            base.AddContract(this.GetType().Name,endpointName, CheckEndpointName());
        }

        public void AddEndpointIdentityContract(string identity)
        {
            base.AddContract(this.GetType().Name, identity, CheckIdentity());
        }

        protected virtual Func<string, bool> CheckEndpointName()
        {
            
            Func<string, bool> checkFuc = (endPointName) =>
            !Util.AssistClass.IsNull(endPointName) &&
            new Regex(_endpointNameNotAllowedRegex).IsMatch(endPointName)
            ;
            return checkFuc;
        }


        protected virtual Func<string, bool> CheckIdentity()
        {
            Guid guidIdentity = Guid.NewGuid();
            Func<string, bool> checkFuc = (identity) =>
            !Util.AssistClass.IsNull(identity)
               && Guid.TryParse(identity, out guidIdentity);
            return checkFuc;
        }
    }
}

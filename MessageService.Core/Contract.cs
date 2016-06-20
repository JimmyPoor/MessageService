using MessageService.Core.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessageService.Core
{
    /// <summary>
    /// check properties value;
    /// </summary>
    public  class Contract
    {
       static Dictionary<string, Dictionary<string, Func<string, bool>>> _contracts =
            new Dictionary<string, Dictionary<string, Func<string, bool>>>();

        public  Task AddContract(string classKey, string propKey, Func<string, bool> contractTask)
        {
            AssistClass.ExceptionWhenNull(contractTask);
            if (!_contracts.ContainsKey(classKey))
            {
                var inner = new Dictionary<string, Func<string, bool>>() { { propKey, contractTask } };
                _contracts.Add(classKey, inner);
            }
            else
            {
              _contracts[classKey][propKey]= contractTask;
            }
            return Task.FromResult(0);
        }

        public virtual  IEnumerable<bool> Invoke(string classkey)
        {
            var propContracts = _contracts[classkey];
             foreach(var propContract in propContracts)
            {
                yield return propContract.Value(propContract.Key);
            }
        }


        public virtual IEnumerable<bool> InvokeAll()
        {
            var temp = new List<bool>() ;
            foreach (var contracts in _contracts)
            {
                temp.AddRange( Invoke(contracts.Key));
            }
             return temp;
        }


    }
}

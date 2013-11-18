using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.Common;
using Kiss.Core;

namespace Kiss.Data
{
    /// <summary>
    /// output/return value parameters
    /// </summary>
    public interface IExecuteResult
    {
        IDictionary Output();
        object ReturnValue();
    }

    public struct ExecuteResult : IExecuteResult
    {
        private Dictionary<string, object> outputParameters;
        private object returnParameter;
        private char parameterPrefix;

        internal ExecuteResult(DbParameterCollection parameters, char prefix)
        {
            returnParameter = null;            
            outputParameters = null;
            parameterPrefix = prefix;

            if (parameters == null)
            {
                return;
            }
            foreach (DbParameter p in parameters)
            {
                if(p.Direction == System.Data.ParameterDirection.Output || p.Direction == System.Data.ParameterDirection.InputOutput)
                {
                    if (outputParameters == null)
                    {
                        outputParameters = new Dictionary<string, object>();
                    }
                    outputParameters[StringUtils.RemovePrefix(p.ParameterName, parameterPrefix)] = p.Value;
                }
                else if(p.Direction == System.Data.ParameterDirection.ReturnValue)
                {
                    returnParameter = p.Value;
                }
            }
        }

        public IDictionary Output()
        {
            return outputParameters;
        }

        public object ReturnValue()
        {
            return returnParameter;
        }
    }
}

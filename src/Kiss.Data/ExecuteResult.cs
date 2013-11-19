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
        int RowsAffected();
    }

    public struct ExecuteResult : IExecuteResult
    {
        private Dictionary<string, object> outputParameters;
        private object returnParameter;
        private int rowsAffected;
        private char parameterPrefix;

        internal ExecuteResult(DbParameterCollection parameters, char prefix, int rowsAffected)
        {
            returnParameter = null;            
            outputParameters = null;
            parameterPrefix = prefix;
            this.rowsAffected = rowsAffected;

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

        public int RowsAffected()
        {
            return rowsAffected;
        }

        public IDictionary Output()
        {
            if (outputParameters == null)
            {
                outputParameters = new Dictionary<string, object>();
            }
            return outputParameters;
        }

        public object ReturnValue()
        {
            return returnParameter;
        }
    }
}

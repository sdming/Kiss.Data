using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Driver
{
    /// <summary>
    /// sql dialecter
    /// </summary>
    public interface IDialecter
    {
        /// <summary>
        /// return sql dialecter name
        /// </summary>
        /// <returns></returns>
        string Name();

        /// <summary>
        /// whether support named paramter, like @p
        /// </summary>
        /// <returns></returns>
        bool SupportNamedParameter();

        /// <summary>
        /// return pefix of parameter, like @, :, ?
        /// </summary>
        /// <returns></returns>
        char ParameterPrefix();
        

        /// <summary>
        /// quote a parameter
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string QuoteParameter(string name);

        /// <summary>
        /// quote sql string 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        string QuoteString(string s);

        /// <summary>
        /// quote identifier, like [foo], "foo"
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string QuoteIdentifer(string name);

        /// <summary>
        /// delimiter to spli sql statement
        /// </summary>
        /// <returns></returns>
        string StatementDelimiter();
    }

}

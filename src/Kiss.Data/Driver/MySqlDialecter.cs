﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Driver
{
    /// <summary>
    /// MySqlDialecter
    /// </summary>
    public class MySqlDialecter : IDialecter
    {
        /// <summary>
        /// return sql dialecter name
        /// </summary>
        /// <returns></returns>
        public string Name()
        {
            return "mysql";
        }

        /// <summary>
        /// whether support named paramter, like @p
        /// </summary>
        /// <returns></returns>
        public bool SupportNamedParameter()
        {
            return true;
        }

        /// <summary>
        /// return pefix of parameter
        /// </summary>
        /// <returns></returns>
        public char ParameterPrefix()
        {
            return '?';
        }

        /// <summary>
        /// quote a parameter
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string QuoteParameter(string name)
        {
            return "?" + name;
        }

        /// <summary>
        /// quote sql string 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string QuoteString(string s)
        {
            return "\"" + s + "\"";
        }


        /// <summary>
        /// quote identifier, like [foo], "foo"
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string QuoteIdentifer(string name)
        {
            return "`" + name + "`";
        }

        /// <summary>
        /// delimiter to spli sql statement
        /// </summary>
        /// <returns></returns>
        public string StatementDelimiter()
        {
            return " ; ";
        }

        //"SELECT LAST_INSERT_ID()"
        //"ROW_COUNT()"
        //"uuid()"
        //" current_time() "
    }
}

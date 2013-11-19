using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Kiss.Data.Schema
{
    /// <summary>
    /// DbProcedure
    /// </summary>
    [Serializable]
    public class SqlProcedure 
    {
        ///// <summary>
        ///// Catalog
        ///// </summary>
        //public string Catalog { get; set; }

        ///// <summary>
        ///// Schema
        ///// </summary>
        //public string Schema { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Parameters
        /// </summary>
        public List<SqlParameter> Parameters { get; set; }

        /// <summary>
        /// SqlProcedure
        /// </summary>
        public SqlProcedure()
        {
            this.Parameters = new List<SqlParameter>();
        }

        /// <summary>
        /// return matched parameter or null
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SqlParameter TryFindParameter(string name)
        {
            return Parameters.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// return matched parameter or throw exception
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public SqlParameter FindParameter(string name)
        {
            var p = TryFindParameter(name);
            if (p == null)
            {
                throw new Exception("can not find parameter " + name);
            }
            return p;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ContainsParameter(string name)
        {
            return TryFindParameter(name) != null;
        }

        /// <summary>
        /// HasOutputParams
        /// </summary>
        /// <returns></returns>
        public bool HasOutputParams()
        {
            foreach (var p in Parameters)
            {
                if (p.Direction != ParameterDirection.Input)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// OutputParameters
        /// </summary>
        public IEnumerable<SqlParameter> OutputParameters
        {
            get
            {
                return from SqlParameter item in Parameters
                       where item.Direction == ParameterDirection.Output
                        || item.Direction == ParameterDirection.InputOutput
                       select item;
            }
        }

        /// <summary>
        /// ReturnParameter
        /// </summary>
        public SqlParameter ReturnParameter
        {
            get
            {
                foreach (var p in Parameters)
                {
                    if (p.Direction != ParameterDirection.ReturnValue)
                    {
                        return p;
                    }
                }
                return null;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{{\"Name\":\"{0}\"", Name);
            sb.Append(",\"Parameters\":[ ");

            bool split = false;
            if (Parameters != null)
            {
                foreach (var p in Parameters)
                {
                    if (split)
                    {
                        sb.Append(",");
                    }
                    split = true;
                    sb.Append(p);
                }
            }
            sb.Append("] }");
            return sb.ToString();
        }
    }
}

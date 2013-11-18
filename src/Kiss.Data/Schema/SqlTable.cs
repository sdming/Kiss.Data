using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Kiss.Data.Schema
{
    [Serializable]
    public class SqlTable 
    {
        //public string Catalog { get; set; }
        //public string Schema { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Columns
        /// </summary>
        public List<SqlColumn> Columns { get; set; }

        /// <summary>
        /// SqlTable
        /// </summary>
        public SqlTable()
        {
            this.Columns = new List<SqlColumn>();
        }

        /// <summary>
        /// FindColumn
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SqlColumn FindColumn(string name)
        {
            return this.Columns.FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase)); 
        }

        /// <summary>
        /// PrimaryKey
        /// </summary>
        public SqlColumn Key()
        {
            return this.Columns.FirstOrDefault(c => c.IsKey); 
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{{\"Name\":\"{0}\"", Name);
            sb.Append(",\"Columns\":[ ");

            bool split = false;
            if(Columns != null)
            {
                foreach(var col in Columns)
                {
                    if (split)
                    {
                        sb.Append(",");
                    }
                    split = true;
                    sb.Append(col);                    
                }
            }
            sb.Append("] }");
            return sb.ToString();
        }
    }
}

        

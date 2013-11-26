using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Dynamic;

namespace Kiss.Data
{
    /// <summary>
    /// DataExtension
    /// </summary>
    public static class DataExtension
    {
        public static List<dynamic> ToExpandoList(this IDataReader reader)
        {
            var result = new List<dynamic>();
            while (reader.Read())
            {
                result.Add(reader.ToExpando());
            }
            return result;
        }

        public static DataTable ToTable(this IDataReader reader)
        {
            var table = Utils.Read2Table(reader);
            return table;
        }

        public static dynamic ToExpando(this IDataReader reader)
        {
            dynamic obj = new ExpandoObject();
            var d = obj as IDictionary<string, object>;
            for (int i = 0; i < reader.FieldCount; i++)
            {
                d.Add(reader.GetName(i), DBNull.Value.Equals(reader[i]) ? null : reader[i]);
            }
            return obj;
        }

        public static object ToScalar(this IDataReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (reader.IsClosed || reader.FieldCount == 0)
            {
                throw new Exception("reader is closed or reader does not has fields");
            }

            if (reader.FieldCount <=0 )
            {
                throw new Exception(string.Format("field count is :{0}", reader.FieldCount));
            }

            object v = null;
            while(reader.Read())
            {
                if(!reader.IsDBNull(0))
                {
                    v = reader.GetValue(0);
                }
            }
            return v;
        }

        public static int ColumnHash(IDataReader reader)
        {
            //The C Programming Language
            long seed = 131;
            long hash = reader.FieldCount;
            for (int i = 0; i < reader.FieldCount; i++)
            {
                hash = (hash * seed) + reader.GetName(i).GetHashCode();
            }
            return hash.GetHashCode();              
           
        }
    }
}

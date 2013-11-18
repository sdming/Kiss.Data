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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace Kiss.Data
{
    public sealed class Utils
    {
        /// <summary>
        /// return columns of IDataReader
        /// </summary>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        public static DataColumn[] ReaderColumns(IDataReader dataReader)
        {
            DataColumn[] cols = new DataColumn[dataReader.FieldCount];
            for (int i = 0; i < cols.Length; i++)
            {
                cols[i] = new DataColumn(dataReader.GetName(i), dataReader.GetFieldType(i));
            }
            return cols;
        }

        /// <summary>
        /// load IDataReader to DataTable
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:丢失范围之前释放对象")]
        public static DataTable Read2Table(IDataReader reader)
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(ReaderColumns(reader));
            dt.BeginLoadData();
            try
            {
                while (reader.Read())
                {
                    object[] values = new object[reader.FieldCount];
                    reader.GetValues(values);
                    dt.Rows.Add(values);
                }
            }
            finally
            {
                reader.Close();
            }
            dt.EndLoadData();
            return dt;
        }

        /// <summary>
        /// dump cbcommand info
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static string Dump(DbCommand command)
        {            
            StringBuilder sb = new StringBuilder();
            sb.Append("CommandText:");
            sb.Append(command.CommandText);
            sb.Append(",");
            sb.Append("Parameters:");
            sb.Append("[");

            bool split = false;
            foreach (DbParameter p in command.Parameters)
            {
                if (split)
                {
                    sb.Append(",");
                }
                else
                {
                    split = true;
                }
                sb.Append("{");
                sb.AppendFormat("Name:{0}, Value:{1}", p.ParameterName, p.Value);
                sb.Append("}");
            }
            sb.Append("]");

            return sb.ToString();
        }

        /// <summary>
        /// dump DataTable
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string Dump(DataTable table)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("table:{0};rows:{1}; columns:{2};\r\n", table.TableName, table.Rows.Count, table.Columns.Count);
            int fieldCount = table.Columns.Count;

            for (int i = 0; i < fieldCount; i++)
            {
                sb.Append(table.Columns[i].ColumnName);
                sb.Append("\t\t");
            }
            sb.Append("\r\n");
            for (int i = 0; i < fieldCount; i++)
            {
                sb.Append("--------");
            }
            sb.Append("\r\n");

            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < fieldCount; i++)
                {
                    sb.Append(row[i]);
                    sb.Append("\t");
                }
                sb.Append("\r\n");
            }

            return sb.ToString();
        }

        /// <summary>
        /// return string of obj, format as [{key:value}]
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static string PrintDictionary(IDictionary obj)
        {
            if (obj == null)
            {
                return " null ";
            }

            StringBuilder sb = new StringBuilder();
            bool first = true;

            sb.Append("[");
            foreach (DictionaryEntry o in obj)
            {
                if (!first)
                {
                    sb.Append(", ");
                }
                else
                {
                    first = false;
                }

                if (o.Value == null)
                {
                    sb.AppendFormat("{{{0}:null}}", o.Key);
                }
                else
                {
                    sb.AppendFormat("{{{0}:{1} }}", o.Key, Print(o));
                }
            }

            sb.Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Print
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static string Print(object obj)
        {
            if (obj == null)
            {
                return " null ";
            }
            var l = obj as IList;
            if (l != null)
            {
                return PrintList(l);
            }

            var k = obj as IDictionary;
            if (k != null)
            {
                return PrintDictionary(k);
            }

            if(obj is DateTime )
            {
                return PrintDateTime((DateTime)obj);
            }
            return string.Format("{0}", obj);
        }

        /// <summary>
        /// return string of datetime, format:yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static string PrintDateTime(DateTime obj)
        {
            return obj.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// return string of list, format as [x,x]
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static string PrintList(IList obj)
        {
            if (obj == null)
            {
                return " null ";
            }

            StringBuilder sb = new StringBuilder();
            bool first = true;

            sb.Append("[");
            foreach (var o in obj)
            {
                if (!first)
                {
                    sb.Append(", ");
                }
                else
                {
                    first = false;
                }
                if (o == null)
                {
                    sb.Append("null");
                }
                else
                {
                    sb.Append(Print(o));
                }
            }

            sb.Append("]");
            return sb.ToString();
        }
    }
}

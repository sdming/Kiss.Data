using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kiss.Data.Schema;
using Kiss.Data.Expression;
using Kiss.Core;
using System.Data.Common;
using System.Data;

namespace Kiss.Data
{
    /// <summary>
    /// single table access
    /// </summary>
    public struct TableGate
    {
        private DbContent content;
        private bool ignoreSchema;
        private SqlTable schema;
        private string tableName;
        private int parameterIndex;

        /// <summary>
        /// TableGate
        /// </summary>
        /// <param name="content"></param>
        /// <param name="tableName"></param>
        /// <param name="schema"></param>
        internal TableGate(DbContent content, string tableName, SqlTable schema)
        {
            this.content = content;
            this.tableName = tableName;
            this.schema = schema;
            ignoreSchema = schema == null;
            parameterIndex = 0;

            if (ignoreSchema)
            {
                return;
            }
        }

        #region Exists

        public bool Exists(string whereColumn, object whereValue)
        {
            return Exists(whereColumn, SqlOperator.EqualsTo, whereValue, null, null, null);
        }

        public bool Exists(string whereColumn, SqlOperator whereOp, object whereValue)
        {
            return Exists(whereColumn, whereOp, whereValue, null, null, null);
        }

        public bool Exists(string whereColumn1, object whereValue1, string whereColumn2, object whereValue2)
        {
            return Exists(whereColumn1, SqlOperator.EqualsTo, whereValue1, whereColumn2, SqlOperator.EqualsTo, whereValue2);
        }

        public bool Exists(string whereColumn1, SqlOperator whereOp1, object whereValue1, string whereColumn2, SqlOperator whereOp2, object whereValue2)
        {
            return ReadCount(whereColumn1, whereOp1, whereValue1, whereColumn2, whereOp2, whereValue2) > 0;
        }

        #endregion

        #region delete

        public int Delete(string whereColumn, object whereValue)
        {
            return Delete(whereColumn, SqlOperator.EqualsTo, whereValue, null, null, null);
        }

        public int Delete(string whereColumn, SqlOperator whereOp, object whereValue)
        {
            return Delete(whereColumn, whereOp, whereValue, null, null, null);
        }

        public int Delete(string whereColumn1, object whereValue1, string whereColumn2, object whereValue2)
        {
            return Delete(whereColumn1, SqlOperator.EqualsTo, whereValue1, whereColumn2, SqlOperator.EqualsTo, whereValue2);
        }

        public int Delete(string whereColumn1, SqlOperator whereOp1, object whereValue1, string whereColumn2, SqlOperator whereOp2, object whereValue2)
        {
            return Delete(BuildWhere(whereColumn1, whereOp1, whereValue1, whereColumn2, whereOp2, whereValue2));
        }

        public int Delete(Where where)
        {
            Delete exp = new Expression.Delete(tableName);
            exp.Where = where;
            return content.ExecuteNonQuery(exp);
        }

        #endregion

        #region query

        public object ReadCell(string column, string whereColumn, object whereValue)
        {
            return ReadCell(column, whereColumn, whereValue, null, null);
        }

        public object ReadCell(string column, string whereColumn1, object whereValue1, string whereColumn2, object whereValue2)
        {
            Query exp = new Query(tableName);
            exp.Select.Column(column);
            exp.Where = BuildWhere(whereColumn1, SqlOperator.EqualsTo, whereValue1, whereColumn2, SqlOperator.EqualsTo, whereValue2);
            return content.ExecuteScalar(exp);            
        }

        public IList<T> ReadColumn<T>(string column, bool distinct, string whereColumn, object whereValue)
        {
            return ReadColumn<T>(column, distinct, whereColumn, SqlOperator.EqualsTo, whereValue, null, null, null);
        }

        public IList<T> ReadColumn<T>(string column, bool distinct, string whereColumn, SqlOperator whereOp, object whereValue)
        {
            return ReadColumn<T>(column, distinct, whereColumn, whereOp, whereValue, null, null, null);
        }

        public IList<T> ReadColumn<T>(string column, bool distinct, string whereColumn1, object whereValue1, string whereColumn2, object whereValue2)
        {
            return ReadColumn<T>(column, distinct, whereColumn1, SqlOperator.EqualsTo, whereValue1, whereColumn2, SqlOperator.EqualsTo, whereValue2);
        }

        public IList<T> ReadColumn<T>(string column, bool distinct, string whereColumn1, SqlOperator whereOp1, object whereValue1, string whereColumn2, SqlOperator whereOp2, object whereValue2)
        {
            Query exp = new Query(tableName);
            if (distinct)
            {
                exp.Distinct();
            }
            exp.Select.Column(column);
            exp.Where = BuildWhere(whereColumn1, whereOp1, whereValue1, whereColumn2, whereOp2, whereValue2);
            IDataReader reader = content.ExecuteReader(exp);
            return DataConvert.ToList<T>(reader);
        }

        public int ReadCount(string whereColumn, object whereValue)
        {
            return ReadCount(whereColumn, SqlOperator.EqualsTo, whereValue, null, null, null);
        }

        public int ReadCount(string whereColumn, SqlOperator whereOp, object whereValue)
        {
            return ReadCount(whereColumn, whereOp, whereValue, null, null, null);
        }

        public int ReadCount(string whereColumn1, object whereValue1, string whereColumn2, object whereValue2)
        {
            return ReadCount(whereColumn1, SqlOperator.EqualsTo, whereValue1, whereColumn2, SqlOperator.EqualsTo, whereValue2);
        }

        public int ReadCount(string whereColumn1, SqlOperator whereOp1, object whereValue1, string whereColumn2, SqlOperator whereOp2, object whereValue2)
        {
            Query exp = new Query(tableName);
            exp.Select.Count(null, "count_number");
            exp.Where = BuildWhere(whereColumn1, whereOp1, whereValue1, whereColumn2, whereOp2, whereValue2);
            return Convert.ToInt32(content.ExecuteScalar(exp));
        }

        public IDataReader Read(string whereColumn, object whereValue)
        {
            return Read(whereColumn, SqlOperator.EqualsTo, whereValue, null, null, null);
        }

        public IDataReader Read(string whereColumn, SqlOperator whereOp, object whereValue)
        {
            return Read(whereColumn, whereOp, whereValue, null, null, null);
        }

        public IDataReader Read(string whereColumn1, object whereValue1, string whereColumn2, object whereValue2)
        {
            return Read(whereColumn1, SqlOperator.EqualsTo, whereValue1, whereColumn2, SqlOperator.EqualsTo, whereValue2);
        }

        public IDataReader Read(string whereColumn1, SqlOperator whereOp1, object whereValue1, string whereColumn2, SqlOperator whereOp2, object whereValue2)
        {
            return Read(BuildWhere(whereColumn1, whereOp1, whereValue1, whereColumn2, whereOp2, whereValue2));
        }

        public IDataReader Read(Where where)
        {
            Query exp = new Query(tableName);
            exp.Select.All();
            exp.Where = where;
            return content.ExecuteReader(exp);
        }

        #endregion

        #region update
        public int UpdateColumn(string updateColumn, object updateValue, string whereColumn, object whereValue)
        {
            Where where = BuildWhere(whereColumn, SqlOperator.EqualsTo, whereValue, null, null, null);
            return UpdateColumn(updateColumn, updateValue, where);
        }

        public int UpdateColumn(string updateColumn, object updateValue, Where where)
        {
            Update exp = new Update(tableName);

            if (!ignoreSchema)
            {

                var col = schema.FindColumn(updateColumn);
                if (col.IsAutoIncrement || col.IsReadOnly)
                {
                    throw new Exception("column can not update:" + col.Name);
                }

                Parameter p = new Parameter();
                p.Name = col.Name;
                col.Copy(p);
                p.Value = updateValue;
                exp.Set(new Set(col.Name, p));
            }
            else
            {
                exp.Set(updateColumn, updateValue);
            }

            exp.Where = where;
            return content.ExecuteNonQuery(exp);
        }

        public int Update(IDataObjectAdapter data, string whereColumn, object whereValue)
        {
            Where where = BuildWhere(whereColumn, SqlOperator.EqualsTo, whereValue);
            return Update(data, where, null, null, null);
        }

        public int Update(IDataObjectAdapter data, string whereColumn, SqlOperator whereOp, object whereValue)
        {
            Where where = BuildWhere(whereColumn, whereOp, whereValue);
            return Update(data, where, null, null, null);
        }

        public int Update(IDataObjectAdapter data, string whereColumn1, object whereValue1, string whereColumn2, object whereValue2)
        {
            Where where = BuildWhere(whereColumn1, SqlOperator.EqualsTo, whereValue1, whereColumn2, SqlOperator.EqualsTo, whereValue2);
            return Update(data, where, null, null, null);
        }

        public int Update(IDataObjectAdapter data, string whereColumn1, SqlOperator whereOp1, object whereValue1, string whereColumn2, SqlOperator whereOp2, object whereValue2)
        {
            Where where = BuildWhere(whereColumn1, whereOp1, whereValue1, whereColumn2, whereOp2, whereValue2);
            return Update(data, where, null, null, null);
        }

        public int Update(IDataObjectAdapter data, Where where)
        {
            return Update(data, where, null, null, null);
        }

        public int Update(IDataObjectAdapter data, Where where, Func<string, string> mapping, IEnumerable<string> include, IEnumerable<string> exclude)
        {
            Update exp = new Update(tableName);

            if (!ignoreSchema)
            {
                for (var i = 0; i < schema.Columns.Count; i++)
                {
                    var col = schema.Columns[i];
                    if (col.IsAutoIncrement || col.IsReadOnly)
                    {
                        continue;
                    }
                    string name = GetFieldName(col.Name, mapping, include, exclude);
                    if (string.IsNullOrEmpty(name))
                    {
                        continue;
                    }

                    bool exist = data.Contains(name);
                    if (!exist && col.AllowDBNull)
                    {
                        continue;
                    }

                    Parameter p = new Parameter();
                    p.Name = col.Name;
                    col.Copy(p);
                    if (exist)
                    {
                        p.Value = data.Get(name);
                    }
                    else
                    {
                        p.Value = col.DbType.Default();
                    }
                    exp.Set(new Set(col.Name, p));
                }
            }
            else
            {
                foreach (string name in data.Fields())
                {
                    exp.Set(name, data.Get(name));
                }
            }

            exp.Where = where;
            return content.ExecuteNonQuery(exp);
        }

        #endregion
     
        #region insert

        public object Insert(IDataObjectAdapter data)
        {
            return Insert(data, null, null, null);
        }

        public object Insert(IDataObjectAdapter data, Func<string, string> mapping, IEnumerable<string> include, IEnumerable<string> exclude)
        {
            Insert exp = new Insert(tableName);

            if (!ignoreSchema)
            {
                for (var i = 0; i < schema.Columns.Count; i++)
                {
                    var col = schema.Columns[i];
                    if(col.IsAutoIncrement || col.IsReadOnly)
                    {
                        continue;
                    }
                    string name = GetFieldName(col.Name, mapping, include, exclude);
                    if (string.IsNullOrEmpty(name))
                    {
                        continue;
                    }

                    bool exist = data.Contains(name);
                    if (!exist && col.AllowDBNull)
                    {
                        continue;
                    }

                    Parameter p = new Parameter();
                    p.Name = col.Name;
                    col.Copy(p);
                    if (exist)
                    {
                        p.Value = data.Get(name);
                    }
                    else 
                    {
                        p.Value = col.DbType.Default();
                    }
                    exp.Set(new Set(col.Name, p));
                }
            }
            else
            {
                foreach(string name in data.Fields())
                {
                    exp.Set(name, data.Get(name));
                }
            }
            
            return content.ExecuteScalar(exp);
        }

        #endregion

        #region where

        private Where BuildWhere(string column1, SqlOperator op1, object value1)
        {
            Where where = new Where();
            BuildWhereColumn(where, column1, op1, value1);
            return where;
        }

        private Where BuildWhere(string column1, SqlOperator op1, object value1, string column2, SqlOperator op2, object value2)
        {
            Where where = new Where();
            BuildWhereColumn(where, column1, op1, value1);
            BuildWhereColumn(where, column2, op2, value2);
            return where;
        }

        private void BuildWhereColumn(Where where, string column, SqlOperator op, object value)
        {
            if (string.IsNullOrWhiteSpace(column) || op == null)
            {
                return;
            }
            if (ignoreSchema)
            {
                where.Compare(op, column, value);
                return;
            }

            SqlColumn c = schema.FindColumn(column);
            if (c == null)
            {
                throw new Exception(string.Format("can not find column:{0}.{1}", tableName, column));
            }
            Condition d = new Condition();
            d.Op = op;
            d.Left = new Column(c.Name);
            Parameter p = new Parameter();
            parameterIndex++;
            p.Name = string.Concat(c.Name, "_", parameterIndex);
            c.Copy(p);
            p.Value = value;
            d.Right = p;
            where.Append(d);
        }


        #endregion

        #region schema

      

        #endregion

        #region inner
        private string GetFieldName(string columnName, Func<string, string> mapping, IEnumerable<string> include, IEnumerable<string> exclude)
        {
            string fieldName = null;
            if (mapping != null)
            {
                fieldName = mapping(columnName);
                if (string.IsNullOrEmpty(fieldName))
                {
                    return null;
                }
            }
            else
            {
                fieldName = columnName;
            }

            if (include != null && !include.Contains(fieldName, StringComparer.OrdinalIgnoreCase))
            {
                return null;
            }

            if (exclude != null && exclude.Contains(fieldName, StringComparer.OrdinalIgnoreCase))
            {
                return null;
            }

            return fieldName;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Kiss.Data.Expression;
using System.Data;

namespace Kiss.Data.Entity
{
    public class Repository<T> where T:new()
    {
        #region entity
        public string DbName { get; set; }
        public Repository(string dbName)
        {
            this.DbName = dbName;
        }

        public DbContent Content { get; set; }
        public Repository(DbContent content)
        {
            this.Content = content;
        }
        #endregion

        #region methods
        protected static Type entityType;
        protected static Kiss.Core.Reflection.TypeMeta entityMeta;

        static Repository()
        {
            entityType = typeof(T);
            entityMeta = Kiss.Core.Reflection.TypeMetaFactory.CreateTypeMeta(entityType);

            //table
            defaultTableName = entityType.Name;
            
            //mapping
            enablemapping = false;
            var fields = entityMeta.Members();
            string[] names = new string[] { "Id", "PK", "PrimaryKey", "Key", "SysNo", entityType.Name + "Id", entityType.Name + "Key", };

            foreach (var f in fields)
            {
                string columnName = f.Name;
                fieldToColumnMappping[f.Name] = columnName;
                columnToFieldMappping[columnName] = f.Name;

                if (string.IsNullOrEmpty(fieldKey))
                {
                    fieldKey = names.FirstOrDefault((x) => string.Equals(x, f.Name, StringComparison.CurrentCultureIgnoreCase));
                }
            }
        }

        protected static string defaultTableName;
        public virtual string TableName { get; set; }

        public virtual string GetTableName()
        {
            if (!string.IsNullOrEmpty(TableName))
            {
                return TableName;
            }
            return defaultTableName;
        }

        public static void RegisterTableName(string table)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            defaultTableName = table;
        }

        protected virtual void Execute(Action<DbContent> action)
        {
            if (this.Content != null)
            {
                action(this.Content);
                return;
            }

            using (DbContent db = new DbContent(this.DbName))
            {
                action(this.Content);
                return;
            }
        }

        protected virtual void ExecuteTable(Action<TableGate> action)
        {
            if (this.Content != null)
            {
                var t = this.Content.Table(GetTableName());
                action(t);
                return;
            }

            using (DbContent db = new DbContent(this.DbName))
            {
                var t = this.Content.Table(GetTableName());
                action(t);
                return;
            }
        }

        protected virtual Where CompileWhere(Expression<Func<T, bool>> where)
        {
            if (where == null)
            {
                return null;
            }

            LambdaVisitor<T> visitor = new LambdaVisitor<T>();
            return visitor.Compile(where, MapFieldToColumnFunc());
        }

        protected virtual Dictionary<string, object> BuildWhere(string whereField1, object whereValue1, string whereField2, object whereValue2)
        {
            Dictionary<string, object> where = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrEmpty(whereField1))
            {
                AppendField(where, whereField1, whereValue1);
            }
            if (!string.IsNullOrEmpty(whereField2))
            {
                AppendField(where, whereField2, whereValue2);
            }

            return where;
        }

        protected virtual void AppendField(Dictionary<string, object> data, Expression<Func<T, object>> field, object value)
        {
            if (field == null)
            {
                return;
            }
            var name = GetFieldName(field);
            if (string.IsNullOrEmpty(name))
            {
                return;
            }
            string column = MapFieldToColumn(name);
            if (string.IsNullOrEmpty(column))
            {
                return;
            }
            data[column] = value;
        }

        protected virtual void AppendField(Dictionary<string, object> data, string field, object value)
        {
            string column = MapFieldToColumn(field);
            if (string.IsNullOrEmpty(column))
            {
                return;
            }
            data[column] = value;
        }

        protected virtual IList<T> ConvertToList(IDataReader reader)
        {
            if (reader == null)
            {
                return default(List<T>);
            }

            var convert = CreateConvert(reader);
            return convert.Convert(reader);
        }

        protected static DataReaderConvert<T> convert = null;
        public static DataReaderConvert<T> CreateConvert(IDataReader reader)
        {
            if (convert == null)
            {
                convert = new DataReaderConvert<T>(reader, MapColumnToFieldFunc());
            }
            return convert;
        }

        protected static Dictionary<string, string> fieldToColumnMappping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        protected static Dictionary<string, string> columnToFieldMappping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        protected static bool enablemapping = false;
        public static void RegisterMapping(string field, string column)
        {
            if (string.IsNullOrEmpty(field))
            {
                throw new ArgumentNullException("field");
            }
            if (string.IsNullOrEmpty(column))
            {
                throw new ArgumentNullException("column");
            }

            enablemapping = true;
            fieldToColumnMappping[field] = column;
            fieldToColumnMappping[column] = field;
        }

        public static Func<string, string> MapColumnToFieldFunc()
        {
            if (!enablemapping)
            {
                return null;
            }
            return MapColumnToField;
        }

        public static Func<string, string> MapFieldToColumnFunc()
        {
            if (!enablemapping)
            {
                return null;
            }
            return MapFieldToColumn;
        }

        public static string MapColumnToField(string column)
        {
            if (!enablemapping)
            {
                return column;
            }
            string field;
            if (columnToFieldMappping.TryGetValue(column, out  field))
            {
                return field;
            }
            return null;
        }

        public static string MapFieldToColumn(string field)
        {
            if (!enablemapping)
            {
                return field;
            }
            string column;
            if (fieldToColumnMappping.TryGetValue(field, out  column))
            {
                return column;
            }
            return null;
        }

        protected static string fieldKey;
        public static void RegisterKey(string nameOfKey)
        {
            fieldKey = nameOfKey;
        }

        protected static HashSet<string> notUpdateAbleFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public static void RegisterNotUpdateAble(string name, params string[] names)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("name");
            }
            HashSetAdd(notUpdateAbleFields, name, names);
        }

        protected static HashSet<string> notInsertAbleFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public static void RegisterNotInsertAble(string name, params string[] names)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("name");
            }
            HashSetAdd(notInsertAbleFields, name, names);
        }

        protected static void HashSetAdd(HashSet<string> data, string item, params string[] items)
        {
            if (!string.IsNullOrEmpty(item) && !data.Contains(item))
            {
                data.Add(item);
            }

            if (items != null)
            {
                foreach (var n in items)
                {
                    if (!string.IsNullOrEmpty(n) && !data.Contains(n))
                    {
                        data.Add(n);
                    }
                }
            }
        }

        public virtual string GetFieldKey()
        {
            if (string.IsNullOrEmpty(fieldKey))
            {
                throw new Exception("key is invalid");
            }
            return fieldKey;
        }

        protected static string GetFieldName(Expression<Func<T, object>> field)
        {
            if (field == null)
            {
                return null;
            }
            if (field.NodeType != ExpressionType.Lambda)
            {
                throw new Exception(string.Format("GetFieldName: unhandled expression type: '{0}'", field));
            }

            MemberExpression m = null;
            if (field.Body.NodeType == ExpressionType.Convert)
            {
                m = ((UnaryExpression)field.Body).Operand as MemberExpression;
            }
            else if (field.Body.NodeType == ExpressionType.MemberAccess)
            {
                m = field.Body as MemberExpression;
            }

            if (m == null)
            {
                throw new Exception(string.Format("GetFieldName: unhandled expression type: '{0}'", field.NodeType));
            }
            return m.Member.Name;
        }

        #endregion

        #region update
        public virtual int UpdateByKey(T data, object keyValue)
        {
            string keyName = GetFieldKey();
            return InnerUpdateByFields(data, keyName, keyValue, null, null);
        }

        public virtual int UpdateByFields(T data, Expression<Func<T, object>> whereField, object whereValue)
        {
            return UpdateByFields(data, whereField, whereValue, null, null);
        }

        public virtual int UpdateByFields(T data, Expression<Func<T, object>> whereField1, object whereValue1, Expression<Func<T, object>> whereField2, object whereValue2)
        {
            string whereName1 = GetFieldName(whereField1);
            string whereName2 = GetFieldName(whereField2);
            return InnerUpdateByFields(data, whereName1, whereValue1, whereName2, whereValue2);
        }

        protected virtual int InnerUpdateByFields(T data, string whereField1, object whereValue1, string whereField2, object whereValue2)
        {
            var where = BuildWhere(whereField1, whereValue1, whereField2, whereValue2);
            int i = -1;
            ExecuteTable((x) => i = x.Update(Kiss.Core.Adapter.Object(data), where, MapColumnToFieldFunc(), null, notUpdateAbleFields));
            return i;
        }

        public virtual int UpdateFieldsByKey(object keyValue, Expression<Func<T, object>> field, object value)
        {
            return UpdateFieldsByKey(keyValue, field, value, null, null);
        }

        public virtual int UpdateFieldsByKey(object keyValue, Expression<Func<T, object>> field1, object value1, Expression<Func<T, object>> field2, object value2)
        {
            Dictionary<string, object> data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            AppendField(data, field1, value1);
            AppendField(data, field2, value2);
            return InnerUpdateFieldsByKey(keyValue, data);
        }

        protected virtual int InnerUpdateFieldsByKey(object keyValue, IDictionary<string, object> data)
        {
            int i = -1;
            string keyName = GetFieldKey();
            var where = BuildWhere(keyName, keyValue, null, null);
            ExecuteTable((x) => i = x.Update(Kiss.Core.Adapter.Dictionary(data), where, null, null, null));
            return i;
        }

        public virtual int UpdateFields(Expression<Func<T, bool>> where, Expression<Func<T, object>> field, object value)
        {
            return UpdateFields(where, field, value, null, null);
        }

        public virtual int UpdateFields(Expression<Func<T, bool>> where, Expression<Func<T, object>> field1, object value1, Expression<Func<T, object>> field2, object value2)
        {
            Dictionary<string, object> data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            AppendField(data, field1, value1);
            AppendField(data, field2, value2);
            return UpdateFields(where, data);
        }

        protected virtual int InnerUpdateFields(Expression<Func<T, bool>> where, IDictionary<string, object> fields)
        {
            var w = CompileWhere(where);
            int i = -1;
            ExecuteTable((x) => i = x.Update(Kiss.Core.Adapter.Dictionary(fields), w));
            return i;
        }

        public virtual int UpdateFields(Expression<Func<T, bool>> where, IDictionary<string, object> fields)
        {
            if (!enablemapping)
            {
                return InnerUpdateFields(where, fields);
            }
            Dictionary<string, object> data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var f in fields)
            {
                string name = MapFieldToColumn(f.Key);
                data[name] = f.Value;
            }
            return InnerUpdateFields(where, fields);
        }

        #endregion

        #region add
        public virtual object Add(T data)
        {
            return Add(data, fieldKey);
        }

        public virtual object Add(T data, string output)
        {
            object i = null;
            ExecuteTable((x) => i = x.Insert(Kiss.Core.Adapter.Object(data), MapColumnToFieldFunc(), null, notInsertAbleFields, output));
            return i;
        }

        #endregion

        #region delete
        public virtual int DeleteByKey(object keyValue)
        {
            string keyName = GetFieldKey();
            return InnerDeleteByFields(keyName, keyValue, null, null);
        }

        public virtual int DeleteByFields(Expression<Func<T, object>> whereField, object whereValue)
        {
            return DeleteByFields(whereField, whereValue, null, null);
        }

        public virtual int DeleteByFields(Expression<Func<T, object>> whereField1, object whereValue1, Expression<Func<T, object>> whereField2, object whereValue2)
        {
            string whereName1 = GetFieldName(whereField1);
            string whereName2 = GetFieldName(whereField2);
            return InnerDeleteByFields(whereName1, whereValue1, whereName2, whereValue2);
        }

        protected virtual int InnerDeleteByFields(string whereField1, object whereValue1, string whereField2, object whereValue2)
        {            
            var where = BuildWhere(whereField1, whereValue1, whereField2, whereValue2);
            int i = -1;
            ExecuteTable((x) => i = x.Delete(where));
            return i;
        }

        public virtual int Delete(Expression<Func<T, bool>> where)
        {
            var w = CompileWhere(where);
            int i = -1;
            ExecuteTable((x) => i = x.Delete(w));
            return i;
        }
        #endregion

        #region query
        public T QueryByKey(object keyValue)
        {
            string keyName = GetFieldKey();
            return InnerQueryByFields(keyName, keyValue, null, null).FirstOrDefault();
        }

        public IList<T> QueryByFields(Expression<Func<T, object>> whereField, object whereValue)
        {
            return QueryByFields(whereField, whereValue, null, null);
        }

        public virtual IList<T> QueryByFields(Expression<Func<T, object>> whereField1, object whereValue1, Expression<Func<T, object>> whereField2, object whereValue2)
        {
            string whereName1 = GetFieldName(whereField1);
            string whereName2 = GetFieldName(whereField2);
            return InnerQueryByFields(whereName1, whereValue1, whereName2, whereValue2);
        }

        protected virtual IList<T> InnerQueryByFields(string whereField1, object whereValue1, string whereField2, object whereValue2)
        {
            var where = BuildWhere(whereField1, whereValue1, whereField2, whereValue2);
            IList<T> list = null;
            ExecuteTable((x) => list = ConvertToList(x.Read(where)));
            return list;
        }

        public IList<T> Query(Expression<Func<T, bool>> where)
        {
            var w = CompileWhere(where);
            IList<T> list = null;
            ExecuteTable((x) => list = ConvertToList(x.Read(w)));
            return list;
        }

        #endregion
    }
}

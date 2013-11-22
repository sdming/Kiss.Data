using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Kiss.Core.DataAdapter;
using Kiss.Data.Expression;
using Kiss.Core;

namespace Kiss.Data.Entity
{
    public class ActiveEntity<T> where T : new()
    {
        #region entity
        public string DbName { get; set; }
        public ActiveEntity(string dbName)
        {
            this.DbName = dbName;
        }

        public DbContent Content { get; set; }
        public ActiveEntity(DbContent content)
        {
            this.Content = content;
        }
        #endregion

        #region methods
        protected static Type entityType;
        protected static Kiss.Core.Reflection.TypeMeta entityMeta;
        static ActiveEntity()
        {
            entityType = typeof(T);
            entityMeta = Kiss.Core.Reflection.TypeMetaFactory.CreateTypeMeta(entityType);
            
            //table
            tableName = entityType.Name;
            var tables = entityType.GetCustomAttributes(typeof(DbTableAttribute), false);
            if (tables != null && tables.Length > 0)
            {
                tableName = ((DbTableAttribute)tables[0]).Name;
            }

            //mapping
            enablemapping = false;
            var fields = entityMeta.Members();
            foreach (var f in fields)
            {
                string columnName = f.Name;
                var columns = f.MemberInfo.GetCustomAttributes(typeof(DbColumnAttribute), true);
                if (columns != null && columns.Length > 0)
                {
                    var c = (DbColumnAttribute)columns[0];
                    if (!string.IsNullOrEmpty(c.Name))
                    {
                        enablemapping = true;
                        columnName = c.Name;                        
                    }
                    if (c.IsKey)
                    {
                        fieldKey = f.Name;
                    }
                    if (!c.UpdateAble)
                    {
                        HashSetAdd(notUpdateAbleFields, f.Name);
                    }
                    if (!c.InsertAble)
                    {
                        HashSetAdd(notInsertAbleFields, f.Name);
                    }
                }

                fieldToColumnMappping[f.Name] = columnName;
                columnToFieldMappping[columnName] = f.Name;
            }

            //key
            if (string.IsNullOrEmpty(fieldKey))
            {
                string[] names = new string[] { "Id", "Key", entityType.Name + "Id", entityType.Name + "Key", "PK", "PrimaryKey" };
                foreach (var name in names)
                {
                    var m = entityMeta.TryGetMember(name);
                    if (m != null)
                    {
                        fieldKey = m.Name;
                    }
                }
            }
            

        }

        protected static string tableName;
        protected static Func<string> tableNameFunc;

        public static string GetTableName()
        {
            if (tableNameFunc != null)
            {
                return tableNameFunc();
            }
            return tableName;
        }

        public static void RegisterTableNameFunc(Func<string> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }
            tableNameFunc = func;
        }

        public static void RegisterTableName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            tableName = name;
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
            if(field == null)
            {
                return;
            }
            var name = GetFieldName(field);
            if(string.IsNullOrEmpty(name))
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
            if(string.IsNullOrEmpty(column))
            {
                return;
            }
            data[column] = value;
        }

        protected virtual IList<T> ConvertToList(IDataReader reader)
        {
            if (reader == null )
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
                convert = new DataReaderConvert<T>(reader, MapColumnToFieldFunc() );
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
            if(!enablemapping)
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

        public virtual string GetFieldKey()
        {
            if (string.IsNullOrEmpty(fieldKey))
            {
                throw new Exception("key is invalid");
            }
            return fieldKey;
        }

        protected static HashSet<string> notUpdateAbleFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public static void RegisterNotUpdateAble(string name, params string[] names)
        {
            if(string.IsNullOrEmpty(name))
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
        //Update_A_And_B_By_C_And_D

        public static Action<ActiveEntity<T>, object, object> BeforeUpdate;
        public static Action<ActiveEntity<T>, object, object> AfterUpdate; 

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
            if (BeforeUpdate != null)
            {
                BeforeUpdate(this, data, where);
            }
            ExecuteTable((x) => i = x.Update(Kiss.Core.Adapter.Object(data), where, MapColumnToFieldFunc(), null, notUpdateAbleFields));
            if (AfterUpdate != null)
            {
                AfterUpdate(this, data, where);
            }
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
            if (BeforeUpdate != null)
            {
                BeforeUpdate(this, data, where);
            }
            ExecuteTable((x) => i = x.Update(Kiss.Core.Adapter.Dictionary(data), where, null, null, null));
            if (AfterUpdate != null)
            {
                AfterUpdate(this, data, where);
            }
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
            if (BeforeUpdate != null)
            {
                BeforeUpdate(this, fields, where);
            }
            ExecuteTable((x) => i = x.Update(Kiss.Core.Adapter.Dictionary(fields), w));
            if (AfterUpdate != null)
            {
                AfterUpdate(this, fields, where);
            }
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
        //Add_A_And_B

        public static Action<ActiveEntity<T>, object> BeforeAdd;
        public static Action<ActiveEntity<T>, object> AfterAdd;

        public virtual object Add(T data)
        {
            return Add(data, fieldKey);
        }

        public virtual object Add(T data, string output)
        {
            object i = null;

            if (BeforeAdd != null)
            {
                BeforeAdd(this, data);
            }
            ExecuteTable((x) => i = x.Insert(Kiss.Core.Adapter.Object(data), MapColumnToFieldFunc(), null, notInsertAbleFields, output));
            if (AfterAdd != null)
            {
                AfterAdd(this, data);
            }
            return i;
        }

        #endregion

        #region delete
        //DeleteBy_A_And_B

        public static Action<ActiveEntity<T>, object> BeforeDelete;
        public static Action<ActiveEntity<T>, object> AfterDelete; 

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
            if (BeforeDelete != null)
            {
                BeforeDelete(this, where);
            }
            ExecuteTable((x) => i = x.Delete(where));
            if (AfterDelete != null)
            {
                AfterDelete(this, where);
            }
            return i;
        }

        public virtual int Delete(Expression<Func<T, bool>> where)
        {
            var w = CompileWhere(where);
            int i = -1;
            if (BeforeDelete != null)
            {
                BeforeDelete(this, where);
            }
            ExecuteTable((x) => i = x.Delete(w));
            if (AfterDelete != null)
            {
                AfterDelete(this, where);
            }
            return i;
        }
        #endregion

        #region query
        //QueryBy_A_And_B

        public static Action<ActiveEntity<T>, object> BeforeQuery;
        public static Action<ActiveEntity<T>, object> AfterQuery; 

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

            if (BeforeQuery != null)
            {
                BeforeQuery(this, where);
            }
            IList<T> list = null;
            ExecuteTable((x) => list = ConvertToList(x.Read(where)));
            if (AfterQuery != null)
            {
                AfterQuery(this, where);
            }
            return list;
        }

        public IList<T> Query(Expression<Func<T, bool>> where)
        {
            var w = CompileWhere(where);

            if (BeforeQuery != null)
            {
                BeforeQuery(this, where);
            }
            IList<T> list = null;
            ExecuteTable((x) => list = ConvertToList(x.Read(w)));
            if (AfterQuery != null)
            {
                AfterQuery(this, where);
            }
            return list;
        }

        #endregion
    }

    public class DynamicEntity<T> : System.Dynamic.DynamicObject where T : new()
    {
        public override bool TryInvoke(System.Dynamic.InvokeBinder binder, object[] args, out object result)
        {
            
            return base.TryInvoke(binder, args, out result);
        }
    }
}

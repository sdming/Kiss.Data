using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Kiss.Core.Reflection;

namespace Kiss.Core.DataAdapter
{
    public class GenericObjectAdapter : IDataObjectAdapter
    {
        object data;
        TypeMeta typeMeta = null;

        public GenericObjectAdapter(object data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            this.data = data;
            typeMeta = TypeMetaFactory.CreateTypeMeta(data.GetType());
        }

        public void Set(string field, object value)
        {
            typeMeta.SetValue(data, field, value);
        }

        public object Get(string field)
        {
            return typeMeta.GetValue(field, data);
        }

        public bool Contains(string field)
        {
            return typeMeta.Contains(field);
        }

        public IEnumerable<string> Fields()
        {
            foreach (var member in typeMeta.Members())
            {
                yield return member.Name;
            }              
        }
    }
}

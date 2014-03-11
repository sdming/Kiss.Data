using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Core.DataAdapter
{
    public class KeyValueAdapter : IDataObjectAdapter
    {
        private IDictionary<string, string> data;

        public IDictionary<string, string> Data
        {
            get { return this.data; }
        }

        public KeyValueAdapter(IDictionary<string, string> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            this.data = data;
        }

        public void Set(string field, object value)
        {
            if (value == null)
            {
                data[field] = null;
            }
            else
            {
                data[field] = value.ToString();
            }
        }

        public object Get(string field)
        {
            return data[field];
        }

        public IEnumerable<string> Fields()
        {
            return data.Keys;
        }

        public bool Contains(string field)
        {
            return data.ContainsKey(field);
        }
    }
}

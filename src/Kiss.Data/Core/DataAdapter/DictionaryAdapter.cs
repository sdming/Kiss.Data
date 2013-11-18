using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Core.DataAdapter
{
    public class DictionaryAdapter : IDataObjectAdapter
    {
        private Dictionary<string, object> data;

        public Dictionary<string, object> Data
        {
            get { return this.data; }
        }

        public DictionaryAdapter(Dictionary<string, object> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            this.data = data;
        }

        public void Set(string field, object value)
        {
            data[field] = value;
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

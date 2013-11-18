using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kiss.Core.DataAdapter;

namespace Kiss.Core
{
    public sealed class Adapter
    {
        public static IDataObjectAdapter Dictionary()
        {
            Dictionary<string, object> data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            return new DictionaryAdapter(data);
        }

        public static IDataObjectAdapter Dictionary(Dictionary<string, object> data)
        {
            return new DictionaryAdapter(data);
        }

        public object Entity()
        {
            throw new NotImplementedException();
        }

        public object Xml()
        {
            throw new NotImplementedException();
        }
    }
}

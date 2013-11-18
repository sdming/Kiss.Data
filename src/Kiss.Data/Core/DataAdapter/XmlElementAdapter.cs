using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Kiss.Core.DataAdapter
{
    public class XmlElementAdapter : IDataObjectAdapter
    {
        XmlNode data;

        public XmlElementAdapter(XmlNode data)
        {
            if(data == null)
            {
                throw new ArgumentNullException("data");
            }

            this.data = data;
        }

        public void Set(string field, object value)
        {
            if (value == null)
            {
                data.SelectSingleNode(".//" + field).InnerText = string.Empty;
            }
            else
            {
                data.SelectSingleNode(".//" + field).InnerText = value.ToString();
            }
        }

        public object Get(string field)
        {
            return data.SelectSingleNode(".//" + field).InnerText;
        }

        public bool Contains(string field)
        {
            return data.SelectSingleNode(".//" + field) != null;
        }

        public IEnumerable<string> Fields()
        {
            foreach (XmlNode node in data.ChildNodes)
            {
                yield return node.Name;
            }
          
        }

    }
}

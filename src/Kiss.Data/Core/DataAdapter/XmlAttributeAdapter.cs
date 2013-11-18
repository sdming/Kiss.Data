using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Kiss.Core.DataAdapter
{

    public class XmlAttributeAdapter : IDataObjectAdapter
    {
        private XmlAttributeCollection data;

        public XmlAttributeAdapter(XmlAttributeCollection data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            this.data = data;
        }

        public XmlAttributeAdapter(XmlNode data)
            :this(data.Attributes)
        {
            
        }

        public void Set(string field, object value)
        {
            if(value == null)
            {
                data[field].InnerText = string.Empty;
            }
            else
            {
                data[field].InnerText = value.ToString();
            }
        }

        public object Get(string field)
        {
            if (data[field] == null)
            {
                return null;
            }

            return data[field].InnerXml;
        }

        public bool Contains(string field)
        {
            return data[field] != null;
        }

        public IEnumerable<string> Fields()
        {
            foreach (XmlAttribute item in data)
            {
                yield return item.Name;
            }            
        }
    }
}



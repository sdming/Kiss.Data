using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Kiss.Core
{
    internal class StringUtils
    {
        /// <summary>
        /// write json string to stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="str"></param>
        internal static void WriteEscapedJsonString(Stream stream, string str)
        {
            var writer = JsonReaderWriterFactory.CreateJsonWriter(stream, Encoding.UTF8);
            writer.WriteString(str);
        }

        /// <summary>
        /// EscapJsonString
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        internal static string EscapJsonString(string str)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                var writer = JsonReaderWriterFactory.CreateJsonWriter(stream, Encoding.UTF8);
                writer.WriteString(str);
                return Encoding.UTF8.GetString(stream.ToArray());                
            }
        }

        /// <summary>
        /// RemovePrefix
        /// </summary>
        /// <param name="s"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        internal static string RemovePrefix(string s, char prefix)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }
            if (s[0] == prefix)
            {
                return s.Substring(1);
            }
            return s;
        }

    }
}

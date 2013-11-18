using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Core
{
    /// <summary>
    /// IDataObjectAdapter
    /// </summary>
    public interface IDataObjectAdapter
    {
        /// <summary>
        /// set value
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        void Set(string field, object value);

        /// <summary>
        /// get value
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        object Get(string field);

        /// <summary>
        /// all field names
        /// </summary>
        IEnumerable<string> Fields();

        /// <summary>
        /// Contains
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        bool Contains(string field);
    }
}

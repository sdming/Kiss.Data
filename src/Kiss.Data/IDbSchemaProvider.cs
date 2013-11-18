using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kiss.Data.Schema;

namespace Kiss.Data
{
    /// <summary>
    /// IDbSchemaProvider
    /// </summary>
    public interface IDbSchemaProvider
    {
        /// <summary>
        /// get procedure schema
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        SqlProcedure GetProcedure(string name);

        /// <summary>
        /// get table/view schema
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        SqlTable GetTable(string name);
    }
}

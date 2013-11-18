using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using Kiss.Data.Expression;

namespace Kiss.Data
{
    /// <summary>
    /// listener dbconent event
    /// </summary>
    public interface IContentListener
    {
        /// <summary>
        /// before command executing
        /// </summary>
        /// <param name="command"></param>
        void Executing(DbContent content, DbCommand command);

        /// <summary>
        /// before ISqlExpression compiling
        /// </summary>
        /// <param name="command"></param>
        void Compiling(DbContent content, ISqlExpression expression);

        /// <summary>
        /// prepare connection
        /// </summary>
        /// <param name="content"></param>
        void Preparing(DbContent content);
        
    }
}

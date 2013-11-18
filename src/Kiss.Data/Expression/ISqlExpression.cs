using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// ISqlExpression
    /// </summary>
    public interface ISqlExpression
    {
        NodeType NodeType();
    }
}

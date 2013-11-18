using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kiss.Data;
using System.Data.Common;
using Kiss.Data.Expression;

namespace Kiss.DataTest
{
    public class ContentListener : IContentListener
    {
        public string CommandText;
        public string ExpressionText;

        public void Executing(DbContent content, DbCommand command)
        {
            CommandText = command.CommandText;
        }

        public void Compiling(DbContent content, ISqlExpression expression)
        {
            ExpressionText = expression.ToString();
        }

        public void Preparing(DbContent content)
        {
            
        }

        public ContentListener Clear()
        {
            this.CommandText = null;
            this.ExpressionText = null;
            return this;
        }

    }
}

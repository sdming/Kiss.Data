using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Expression
{
    /// <summary>
    /// sql fragment
    /// </summary>
    [Serializable]
    public abstract class SqlFragment : IRawSql
    {
        /// <summary>
        /// Name
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// raw sql
        /// </summary>
        internal string Sql { get; set; }

        /// <summary>
        /// GetHashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, this))
            {
                return true;
            }

            SqlFragment compareTo = obj as SqlFragment;
            if (compareTo != null)
            {
                return string.Equals(compareTo.Name, this.Name, StringComparison.OrdinalIgnoreCase);
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// ToSql
        /// </summary>
        /// <returns></returns>
        public string ToSql()
        {
            return Sql;
        }
    }
}

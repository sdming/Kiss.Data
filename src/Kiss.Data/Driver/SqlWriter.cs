using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data.Driver
{
    /// <summary>
    /// SqlWriter
    /// </summary>
    public class SqlWriter
    {
        private const string indent = "\t";
        private StringBuilder builder = new StringBuilder();
        private int depth = 0;

        /// <summary>
        /// Reset
        /// </summary>
        public void Reset()
        {
            builder.Clear();
            depth = 0;
        }

        /// <summary>
        /// SqlWriter
        /// </summary>
        public SqlWriter()
        {
            builder = new StringBuilder();
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="s"></param>
        public void Write(string s)
        {
            builder.Append(s);
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="s"></param>
        /// <param name="s2"></param>
        public void Write(string s1, string s2)
        {
            builder.Append(s1);
            builder.Append(s2);
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="s"></param>
        /// <param name="s2"></param>
        public void Write(string s1, string s2, string s3)
        {
            builder.Append(s1);
            builder.Append(s2);
            builder.Append(s3);
        }


        /// <summary>
        /// WriteN
        /// </summary>
        /// <param name="s"></param>
        /// <param name="s2"></param>
        public void WriteN(string s1, string s2)
        {
            builder.Append(s1);
            builder.Append(" ");
            builder.Append(s2);
        }

        /// <summary>
        /// WriteN
        /// </summary>
        /// <param name="s"></param>
        /// <param name="s2"></param>
        /// <param name="s3"></param>
        public void WriteN(string s1, string s2, string s3)
        {
            builder.Append(s1);
            builder.Append(" ");
            builder.Append(s2);
            builder.Append(" ");
            builder.Append(s3);
        }

        /// <summary>
        /// Comma
        /// </summary>
        public void Comma()
        {
            builder.Append(Ansi.Comma);
            builder.Append(" ");
        }

        /// <summary>
        /// OpenParentheses
        /// </summary>
        public void OpenParentheses()
        {
            builder.Append(Ansi.OpenParentheses);
        }

        /// <summary>
        /// CloseParentheses
        /// </summary>
        public void CloseParentheses()
        {
            builder.Append(Ansi.CloseParentheses);
        }

        /// <summary>
        /// LineBreak
        /// </summary>
        public void LineBreak()
        {
            builder.Append(Ansi.LineBreak);
            for( var i = 0; i < depth; i++)
            {
                builder.Append(indent);
            }
        }

        /// <summary>
        /// IndentOuter
        /// </summary>
        public void IndentOuter()
        {
            if (depth > 0)
            {
                depth--;
            }
        }

        /// <summary>
        /// IndentInner
        /// </summary>
        public void IndentInner()
        {
            depth++;
        }

        /// <summary>
        /// String
        /// </summary>
        /// <returns></returns>
        public string String()
        {
            return builder.ToString();
        }
        

    }
}
    



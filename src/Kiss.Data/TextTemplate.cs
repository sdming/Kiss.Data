using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;

namespace Kiss.Data
{

    public class TextTemplate
    {
        public List<TextParameter> Parameters = new List<TextParameter>();
        public string Text { get; set; }

        private const string pattern = @"([{][^}]*[}])";
        private static Regex parameterRegex = new Regex(pattern);
        private static char[] attributeSplit = new char[]{' '};

        public static TextTemplate Compile(string sql)
        {
            if (string.IsNullOrEmpty(sql))
            {
                throw new ArgumentNullException("sql");
            }

            TextTemplate text = new TextTemplate();
            StringBuilder sb = new StringBuilder();
            
            int start = 0;
            start = sql.IndexOf("{{", start);
            if (start < 0)
            {
                text.Text = sql;
            }
            else
            {
                text.Text = sql.Substring(0, start);
                while (start >= 0)
                {
                    int end = sql.IndexOf("}}", start);
                    if (end <= start)
                    {
                        throw new Exception("invalid text template");
                    }

                    string match = sql.Substring(start + 2, end - start - 2).Trim();
                    if (string.IsNullOrEmpty(match))
                    {
                        throw new Exception("invalid text template");
                    }

                    var attributes = match.Split(attributeSplit, StringSplitOptions.RemoveEmptyEntries);
                    if (attributes.Length <= 0)
                    {
                        throw new Exception("invalid text template");
                    }

                    TextParameter p = new TextParameter();
                    p.Name = attributes[0].Trim();
                    p.Direction = ParameterDirection.Input;
                    string dataType = null;

                    int index = 1;
                    if (attributes.Length > index)
                    {
                        switch (attributes[index].Trim().ToLowerInvariant())
                        {
                            case "in":
                                p.Direction = ParameterDirection.Input;
                                index++;
                                break;
                            case "out":
                                p.Direction = ParameterDirection.Output;
                                index++;
                                break;
                            case "inout":
                                p.Direction = ParameterDirection.InputOutput;
                                index++;
                                break;
                            default:
                                dataType = attributes[index];
                                break;
                        }
                    }
                    if (string.IsNullOrEmpty(dataType) && attributes.Length > index)
                    {
                        dataType = attributes[index];
                    }

                    if (!string.IsNullOrEmpty(dataType))
                    {                      
                        DbType t;
                        if (Enum.TryParse<DbType>(dataType, true, out t))
                        {
                            p.DbType = t;
                        }
                        else
                        {
                            p.ProviderDbType = dataType;
                        }                      
                    }

                    text.Parameters.Add(p);
                    text.Text = string.Concat(text.Text, "{{", p.Name, "}}");
                    start = sql.IndexOf("{{", end);
                    if (start > end)
                    {
                        text.Text = string.Concat(text.Text, sql.Substring(end + 2, start - end - 2));
                    }
                    else if (start < 0 && end < sql.Length - 2)
                    {
                        text.Text = string.Concat(text.Text, sql.Substring(end + 2));
                        break;                        
                    }
                }
            }


            return text;

        }
    }

    public class TextParameter
    {
        public string Name { get; set; }

        public DbType? DbType  { get; set; }

        public string ProviderDbType  { get; set; }

        public ParameterDirection Direction { get; set; }

        public TextParameter()
        {
            this.Direction = ParameterDirection.Input;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Name:{0}", Name);
            if (DbType.HasValue)
            {
                sb.AppendFormat(";DbType:{0}", DbType);
            }
            if (!string.IsNullOrEmpty(ProviderDbType))
            {
                sb.AppendFormat(";ProviderDbType:{0}", ProviderDbType);
            }
            if (Direction != ParameterDirection.Input)
            {
                sb.AppendFormat(";Direction:{0}", Direction);
            }
            return sb.ToString();
        }
    }
}

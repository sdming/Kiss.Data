using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kiss.Data.Expression;

namespace Kiss.Data.XForce
{
    public class SmartQuery
    {
        public static Where ToWhere(IEnumerable<KeyValuePair<string, object>> parameters, bool ignoreNullOrEmpty)
        {
            Where w = new Where();
            if(parameters == null)
            {
                return w;
            }

            foreach(var p in parameters)
            {
                if(string.IsNullOrWhiteSpace(p.Key))
                {
                    continue;
                }

                if (ignoreNullOrEmpty)
                {
                    if (p.Value == null || string.IsNullOrWhiteSpace(p.Value.ToString()))
                    {
                        continue;
                    }
                }

                int index = p.Key.IndexOf('_', 0);
                if(index < 0 )
                {
                    w.EqualsTo(p.Key, p.Value);
                    continue;
                }

                string name = p.Key.Substring(0, index);
                string op = p.Key.Substring(index + 1);

                switch (op.ToLowerInvariant())
                {
                    case "eq":
                        w.EqualsTo(name, p.Value);
                        break;
                    case "gt":
                        w.GreaterThan(name, p.Value);
                        break;
                    case "gteq":
                        w.GreaterOrEquals(name, p.Value);
                        break;
                    case "le":
                        w.LessThan(name, p.Value);
                        break;
                    case "leeq":
                        w.LessOrEquals(name, p.Value);
                        break;
                    case "start":
                        w.Like(name, string.Concat(p.Value, "%"));
                        break;
                    case "like":
                        w.Like(name, p.Value);
                        break;
                    case "cntn":
                        w.Like(name, string.Concat("%" , p.Value, "%"));
                        break;
                    case "end":
                        w.Like(name, string.Concat( "%", p.Value));
                        break;
                    default:
                        break;
                }
            }
            
            return w;

        }
        
    }
}

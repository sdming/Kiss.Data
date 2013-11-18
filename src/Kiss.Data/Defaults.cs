using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data
{
    internal sealed class Defaults
    {
        public static DateTime DateTime = DateTime.Parse("1900-01-01 00:00:00.000");
        public static Guid Guid = default(Guid);
        public const int DateTimeMinYear = 1900;
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Kiss.Core
{
    internal static class SafeNativeMethods
    {
        [DllImport("kernel32")]
        public static extern bool QueryPerformanceCounter(ref Int64 count);

        [DllImport("kernel32")]
        public static extern bool QueryPerformanceFrequency(ref Int64 Frequency);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Kiss.Core
{
    public static class KTimer
    {
        private const string lib = "kernel32";

        private const int microsecond = 1000000;

        public static Int64 Frequency = QueryFrequency();

        /// <summary>
        /// QueryFrequency
        /// </summary>
        /// <returns></returns>
        public static Int64 QueryFrequency()
        {
            Int64 f = 1;
            SafeNativeMethods.QueryPerformanceFrequency(ref f);
            return f;
        }

        /// <summary>
        /// QueryTick
        /// </summary>
        /// <returns></returns>
        public static Int64 QueryTick()
        {
            Int64 tick = 0;
            SafeNativeMethods.QueryPerformanceCounter(ref tick);
            return tick;
        }

        /// <summary>
        /// Microsecond
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static int Microsecond(long start, long end)
        {
            long t = ((end - start) * microsecond) / Frequency;
            return Convert.ToInt32(t);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public static long Elapsed(long start)
        {
            Int64 end = 0;
            SafeNativeMethods.QueryPerformanceCounter(ref end);
            return ((end - start) * microsecond) / Frequency;
        }

    }
}

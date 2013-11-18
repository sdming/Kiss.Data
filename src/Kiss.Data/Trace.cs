using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiss.Data
{
    /// <summary>
    /// Logger
    /// </summary>
    public static class Trace
    {
        /// <summary>
        /// global leg level
        /// </summary>
        public static Tracelevel Level = Tracelevel.None;

        /// <summary>
        /// the logger delegate
        /// </summary>
        public static Action<TraceData> Tracer = null;
    }

    [Serializable]
    public struct TraceData
    {
        public Tracelevel Level;
        public string Source;
        public string Message;
        public string StackTrace;
        public long Duration;

        public override string ToString()
        {
            return string.Format("{{Level:{0},Source:{1},Message:{2},Duration:{3},StackTrace:{4} }}",
                Level, Source, Message, Duration, StackTrace);
        }
    }

    /// <summary>
    /// Tracelevel
    /// </summary>
    public enum Tracelevel
    {
        None    = 0,
        Error   = 2,
        Info    = 4,
        Debug   = 6,
        All     = 10,
    }
}

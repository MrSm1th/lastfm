using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace lastfm
{
    public static class Logger
    {
        static Logger()
        {
            // Let's set up debugging, if we're in debug mode!
            #if DEBUG
            string debugDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string debugFile = System.IO.Path.Combine(debugDir, "winamp-cs-debug.txt");

            var t = new DefaultTraceListener()
            {
                LogFileName = debugFile
            };
            Trace.Listeners.Add(t);
            #endif
        }

        //DefaultTraceListener traceListener;

        //static Logger _instance;
        /// <summary>
        /// Gets the logger instance
        /// </summary>
        //public static Logger I
        //{
        //    get
        //    {
        //        if (_instance == null) _instance = new Logger();
        //        return _instance;
        //    }
        //}

        static string GetTimestamp()
        {
            return DateTime.Now.ToString();
        }

        public static void WriteEmptyLine()
        {
            Trace.WriteLine(string.Empty);
        }

        public static void LogMessage(string message)
        {
            //Trace.TraceInformation(message);
            Trace.WriteLine(message, GetTimestamp() + " - " + "Message");
        }

        public static void LogMessage(object obj)
        {
            Trace.WriteLine(obj, GetTimestamp() + " - " + "Message");
        }

        public static void LogMessage(string message, string category)
        {
            Trace.WriteLine(message, GetTimestamp() + " - " + category);
        }

        public static void LogMessage(object obj, string category)
        {
            Trace.WriteLine(obj, GetTimestamp() + " - " + category);
        }

        public static void LogError(string error)
        {
            //Trace.TraceError(error);
            Trace.WriteLine(error, GetTimestamp() + " - " + "Error");
        }

        public static void LogError(string error, string category)
        {
            Trace.WriteLine(error, GetTimestamp() + " - " + category);
        }

        public static void WriteLine(string message)
        {
            Trace.WriteLine(message);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace lastfm
{
    public class Logger
    {
        Logger()
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

        static Logger _instance;
        /// <summary>
        /// Gets the logger instance
        /// </summary>
        public static Logger I
        {
            get
            {
                if (_instance == null) _instance = new Logger();
                return _instance;
            }
        }

        string GetTimestamp()
        {
            return DateTime.Now.ToString();
        }

        public void WriteEmptyLine()
        {
            Trace.WriteLine(string.Empty);
        }

        public void LogMessage(string message)
        {
            //Trace.TraceInformation(message);
            var d = GetTimestamp();
            Trace.WriteLine(message, d + " - " + "Message");
        }

        public void LogMessage(string message, string category)
        {
            var d = GetTimestamp();
            Trace.WriteLine(message, d + " - " + category);
        }

        public void LogError(string error)
        {
            //Trace.TraceError(error);
            var d = GetTimestamp();
            Trace.WriteLine(error, d + " - " + "Error");
        }

        public void LogError(string error, string category)
        {
            var d = GetTimestamp();
            Trace.WriteLine(error, d + " - " + category);
        }
    }
}
